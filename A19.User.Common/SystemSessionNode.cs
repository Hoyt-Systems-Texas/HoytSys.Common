using System;
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

        /// <summary>
        ///     The session key.
        /// </summary>
        public string SessionKey => _sessionKey;

        private readonly ISystemClient _systemClient;
        private Timer _timer;
        private Timer _extendSession;
        private readonly TimeSpan _retryAfter = new TimeSpan(0, 1, 0);
        private readonly TimeSpan _extendSessionBuffer = new TimeSpan(0, 30, 0);

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
                    var timeSpan = l.SystemSessionInfo.ExpiresOn.Subtract(DateTime.Now).Subtract(_extendSessionBuffer);
                    if (_extendSession != null)
                    {
                        _extendSession.Dispose();
                        _extendSession = null;
                    }
                    _extendSession = new Timer((o) =>
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
                        var timespan = r.SystemSessionInfo.ExpiresOn.Subtract(DateTime.Now).Subtract(_extendSessionBuffer);
                        if (_extendSession != null)
                        {
                            _extendSession.Dispose();
                            _extendSession = null;
                        }
                        _extendSession = new Timer(_ =>
                            {
                                Task.Run(async () => { await this.ChangeState(SystemSessionEvent.SessionExpiring); });
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

        private bool CompareAndExchangeState(SystemSessionState expectedState, SystemSessionState newState)
        {
            log.Trace($"Attempt to change state from {CurrentState}:{expectedState} to {newState}.");
            var eSi = (int) expectedState;
            var nSi = (int) newState;
            return Interlocked.CompareExchange(
                       ref _currentState,
                       nSi,
                       eSi) == eSi;
        }

        private void SetState(SystemSessionState newState)
        {
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