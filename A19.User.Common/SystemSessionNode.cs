using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using A19.Messaging.Common;
using Mrh.Monad;
using NLog;

namespace A19.User.Common
{
    public class SystemSessionNode
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private int _currentState = (int) SystemSessionState.Pending;

        /// <summary>
        ///     The id of the system we are accessing.
        /// </summary>
        public readonly int AccessingSystemId;

        private readonly int _systemId;

        /// <summary>
        ///     The session key to use for authentication
        /// </summary>
        private volatile string _sessionKey;

        /// <summary>
        ///     The pass code for this service.
        /// </summary>
        private readonly string _passCode;

        /// <summary>
        ///     When the session key is expiring.
        /// </summary>
        private DateTime _expires;

        private readonly ISystemClient _systemClient;
        private Timer _timer;
        private Timer _extendSession;
        private readonly TimeSpan _retryAfter = new TimeSpan(0, 1, 0);
        private readonly TimeSpan _extendSessionBuffer = new TimeSpan(0, 30, 0);
        private readonly Subject<string> _sessionChanged = new Subject<string>();

        public SystemSessionNode(
            int accessingSystemId,
            string passCode,
            ISystemClient systemClient,
            int systemId)
        {
            AccessingSystemId = accessingSystemId;
            _passCode = passCode;
            _systemClient = systemClient;
            _systemId = systemId;
        }

        public string SessionKey => _sessionKey;

        public IObservable<string> SessionObservable
        {
            get
            {
                if (CurrentState == SystemSessionState.Pending)
                {
                    Task.Run(async () =>
                    {
                        await this.ChangeState(SystemSessionEvent.Authenticate);
                    });
                }
                return _sessionChanged;
            }
        }

        private async Task ChangeState(SystemSessionEvent @event)
        {
            log.Trace($"Processing the event {@event}.");
            switch (@event)
            {
                case SystemSessionEvent.Authenticate:
                    if (CompareAndExchangeState(SystemSessionState.Pending, SystemSessionState.Authenticating)
                        || CompareAndExchangeState(SystemSessionState.Retrying, SystemSessionState.Authenticating))
                    {
                        await this.AuthenticateService();
                    }

                    break;

                case SystemSessionEvent.SessionExpiring:
                    if (CurrentState == SystemSessionState.Authenticated)
                    {
                        _extendSession.Dispose();
                        _extendSession = null;
                        await ExtendSession();
                    }

                    break;

                case SystemSessionEvent.SessionExpired:
                    // Go back to pending since the session has expired.
                    if (CompareAndExchangeState(SystemSessionState.Authenticated, SystemSessionState.Pending))
                    {
                        await ChangeState(SystemSessionEvent.Authenticate);
                    }

                    break;

                case SystemSessionEvent.Retry:
                    if (CompareAndExchangeState(SystemSessionState.Authenticating, SystemSessionState.PendingRetry))
                    {
                        if (_timer != null)
                        {
                            _timer.Dispose();
                            _timer = null;
                        }

                        _timer = new Timer((o) =>
                        {
                            _timer.Dispose();
                            _timer = null;
                            Task.Run(async () => { await ChangeState(SystemSessionEvent.RetryTimeTimerExpired); });
                        }, null, _retryAfter, _retryAfter);
                    }

                    break;

                case SystemSessionEvent.RetryTimeTimerExpired:
                    if (CompareAndExchangeState(SystemSessionState.PendingRetry, SystemSessionState.Retrying))
                    {
                        await ChangeState(SystemSessionEvent.Authenticate);
                    }

                    break;

                case SystemSessionEvent.Authenticated:
                    SetState(SystemSessionState.Authenticated);
                    break;
            }
        }

        private async Task AuthenticateService()
        {
            try
            {
                await _systemClient.Login(new SystemLoginRq
                {
                    PassCode = _passCode,
                    AccessingSystemId = AccessingSystemId,
                    SystemId = _systemId
                }).Bind(async l =>
                {
                    _sessionKey = l.SystemSessionInfo.SessionId;
                    await this.ChangeState(SystemSessionEvent.Authenticated);
                    _sessionChanged.OnNext(_sessionKey);
                    var timeSpan = l.SystemSessionInfo.ExpiresOn.Subtract(DateTime.Now).Subtract(_extendSessionBuffer);
                    if (_extendSession != null)
                    {
                        _extendSession.Dispose();
                        _extendSession = null;
                    }

                    _extendSession =
                        new Timer(
                            (o) =>
                            {
                                Task.Run(async () => { await this.ChangeState(SystemSessionEvent.SessionExpiring); });
                            }, null, timeSpan, timeSpan);
                    return l.ToResultMonad();
                }).Error(async e =>
                {
                    log.Error($"Failure to login.");
                    await ChangeState(SystemSessionEvent.Retry);
                });
            }
            catch (Exception ex)
            {
                log.Error(ex, ex.Message);
                await ChangeState(SystemSessionEvent.Retry);
            }
        }

        private async Task ExtendSession()
        {
            try
            {
                await _systemClient.Extend(new ExtendSystemSessionRq
                {
                    SessionId = _sessionKey,
                    SystemId = _systemId,
                    AccessSystemId = AccessingSystemId
                }).Bind(r =>
                {
                    if (r.Success)
                    {
                        _expires = r.SystemSessionInfo.ExpiresOn;
                        var timespan = r.SystemSessionInfo.ExpiresOn.Subtract(DateTime.Now)
                            .Subtract(_extendSessionBuffer);
                        if (_extendSession != null)
                        {
                            _extendSession.Dispose();
                            _extendSession = null;
                        }

                        _extendSession =
                            new Timer(
                                _ =>
                                {
                                    Task.Run(async () =>
                                    {
                                        await this.ChangeState(SystemSessionEvent.SessionExpiring);
                                    });
                                }, null, timespan, timespan);
                    }
                    else
                    {
                        Task.Run(async () => { await ChangeState(SystemSessionEvent.SessionExpired); });
                    }

                    return r.ToResultMonad();
                });
            }
            catch (Exception ex)
            {
                log.Error(ex, ex.Message);
                // TODO figure out what to do.
            }
        }

        /// <summary>
        ///     Tries to get the current session id.
        /// </summary>
        /// <param name="sessionId">The id of the session to try and get.</param>
        /// <returns>true if successfully got the session id.</returns>
        public bool TryGetSessionId(out string sessionId)
        {
            switch (CurrentState)
            {
                case SystemSessionState.Authenticated:
                    sessionId = _sessionKey;
                    return false;

                case SystemSessionState.Pending:
                    Task.Run(async () => { await this.ChangeState(SystemSessionEvent.Authenticate); });
                    break;
            }
            sessionId = null;
            return false;
        }

        private bool CompareAndExchangeState(SystemSessionState expectedState, SystemSessionState newState)
        {
            var eSi = (int) expectedState;
            var nSi = (int) newState;
            var result = Interlocked.CompareExchange(
                             ref _currentState,
                             nSi,
                             eSi) == eSi;
            if (result)
            {
                log.Trace($"State change from {expectedState} to {newState}.");
            }

            return result;
        }

        private void SetState(SystemSessionState newState)
        {
            log.Trace($"Change the state to {newState}.");
            Interlocked.MemoryBarrier();
            Volatile.Write(ref _currentState, (int) newState);
        }

        private SystemSessionState CurrentState => (SystemSessionState) Volatile.Read(ref _currentState);

        private enum SystemSessionState
        {
            Pending = 0,
            Authenticating = 1,
            Authenticated = 2,
            PendingRetry = 4,
            Retrying = 5,
        }

        private enum SystemSessionEvent
        {
            Authenticate = 0,
            SessionExpiring = 1,
            RetryTimeTimerExpired = 2,
            Authenticated = 3,
            SessionExpired = 4,
            Retry = 10,
        }
    }
}