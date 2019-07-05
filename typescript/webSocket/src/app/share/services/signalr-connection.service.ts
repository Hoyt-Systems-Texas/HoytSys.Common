import * as signalR from '@aspnet/signalr';
import {Injectable, NgZone} from '@angular/core';
import {environment} from '../../../environments/environment';
import {HubConnection} from '@aspnet/signalr';
import {Observable, Subject} from 'rxjs';
import {IMessageEnvelope, MessageEnvelope} from '../Api/Messaging/IMessageEnvelope';
import {BasicState, doEvt, EventNode, goToState, ignoreEvt, IState} from '../Api/StateMachine/IState';
import {BaseContext} from '../Api/StateMachine/BaseContext';
import Timer = NodeJS.Timer;
import {StateMachine} from '../Api/StateMachine/StateMachine';
import {Message} from '../Api/Messaging/Message';
import {PayloadType} from '../Api/Messaging/PayloadType';
import {PendingMessageService} from './pending-message.service';
import {IResultMonad} from '../Api/Monad/ResultMonad';

const HEARTBEAT_INTERVAL_MS = 5000;
const HEARTBEAT_WAIT_MS = 2000;

const RECONNECT_RETRY = 3000;

export enum ConnectionState {
  NotConnected = 0,
  Connecting = 1,
  Connected = 2,
  ConnectionFailed = 3,
  UserAuthenicate = 4
}

export enum ConnectionEvent {
  ConnectEvt = 0,
  AuthSuccessful = 1,
  ConnectAttemptFailedEvt = 2,
  HeartbeatFailed = 3,
  SendPing = 4,
  PongReceived = 5,
  MessageReceived = 6,
  ReconnectTimeout = 7,
  AuthenticationFailed = 8,
  AuthenticateRequested = 9,
  Reauthenticate = 10,
  UserAuthenticate = 11,
  HeartbeatMissed = 12,
  MessageEnvelopeReceived = 13,
  SendMessageEnvelope = 14,
}

type StateParam = MessageEnvelope | UserLoginRs | UserLoginRq;

class ConnectionCtx extends BaseContext<ConnectionState, ConnectionEvent, StateParam> {

  currentTimer: Timer;
  lastReceivedDate: Date;
  connectionRetryCount = 0;
  hubConnection: HubConnection;
  connectionId: string;
  missedHeartbeats = 0;

  /**
   * The subject for asking a user to authenticate.
   */
  userAuthRequest = new Subject();
  envelopeSubject = new Subject<MessageEnvelope>();
}

class NotConnectedState extends BasicState<ConnectionState, ConnectionEvent, ConnectionCtx, StateParam> {

  events(): EventNode<ConnectionState, ConnectionEvent, ConnectionCtx, StateParam>[] {
    return [
      goToState(ConnectionEvent.ConnectEvt, ConnectionState.Connecting)
    ];
  }

  state(): ConnectionState {
    return ConnectionState.NotConnected;
  }

}

class Connecting extends BasicState<ConnectionState, ConnectionEvent, ConnectionCtx, StateParam> {

  entry(evt: ConnectionEvent, ctx: ConnectionCtx, param?: MessageEnvelope) {
    ctx.hubConnection =  new signalR.HubConnectionBuilder()
      .withUrl(environment.signalRConnection)
      .configureLogging(signalR.LogLevel.Information)
      .build();
    ctx.hubConnection.start().catch(() => {
      ctx.add(ConnectionEvent.ConnectAttemptFailedEvt, null);
    }).then(() => {
      ctx.hubConnection.on('authResponse', (success: boolean, connection: string) => {
        if (success) {
          ctx.connectionId = connection;
          ctx.add(ConnectionEvent.AuthSuccessful, null);
        } else {
          ctx.add(ConnectionEvent.ConnectAttemptFailedEvt, param);
        }
      });
      ctx.hubConnection.on('received', (envelope) => {
        ctx.add(ConnectionEvent.MessageEnvelopeReceived, envelope);
      });
      ctx.hubConnection.on('pong', () => {
        ctx.add(ConnectionEvent.PongReceived, null);
      });
      ctx.add(ConnectionEvent.AuthenticateRequested, null);
    });
  }

  events(): EventNode<ConnectionState, ConnectionEvent, ConnectionCtx, StateParam>[] {
    return [
      goToState(ConnectionEvent.AuthenticateRequested, ConnectionState.UserAuthenicate),
      goToState(ConnectionEvent.ConnectAttemptFailedEvt, ConnectionState.ConnectionFailed),
      doEvt(ConnectionEvent.SendPing, new SendPing()),
      doEvt(ConnectionEvent.PongReceived, new PongReceived()),
      goToState(ConnectionEvent.HeartbeatFailed, ConnectionState.Connecting)
    ];
  }

  state(): ConnectionState {
    return ConnectionState.Connecting;
  }

}

class UserAuthenticate extends BasicState<ConnectionState, ConnectionEvent, ConnectionCtx, StateParam> {

  entry(evt: ConnectionEvent, ctx: ConnectionCtx, param?: MessageEnvelope | UserLoginRs) {
    ctx.userAuthRequest.next();
  }

  events(): EventNode<ConnectionState, ConnectionEvent, ConnectionCtx, StateParam>[] {
    return [
      goToState(ConnectionEvent.AuthSuccessful, ConnectionState.Connected),
      goToState(ConnectionEvent.AuthenticationFailed, ConnectionState.UserAuthenicate),
      ignoreEvt(ConnectionEvent.AuthenticateRequested),
      doEvt(ConnectionEvent.UserAuthenticate, new UserAuthenticateAction())
    ];
  }

  state(): ConnectionState {
    return ConnectionState.UserAuthenicate;
  }
}

class SendPing implements IAction<ConnectionState, ConnectionEvent, ConnectionCtx, StateParam> {

  execute(evt: ConnectionEvent, ctx: ConnectionCtx, param?: StateParam) {
    ctx.hubConnection.send('ping', ctx.connectionId);
    if (ctx.currentTimer) {
      clearTimeout(ctx.currentTimer);
      ctx.currentTimer = null;
    }
    ctx.currentTimer = setTimeout(() => {
      ctx.add(ConnectionEvent.HeartbeatMissed, null);
    }, HEARTBEAT_WAIT_MS);
  }
}

class PongReceived implements IAction<ConnectionState, ConnectionEvent, ConnectionCtx, StateParam> {
  execute(evt: ConnectionEvent, ctx: ConnectionCtx, param?: StateParam) {
    ctx.lastReceivedDate = new Date();
    if (ctx.currentTimer) {
      clearTimeout(ctx.currentTimer);
      ctx.currentTimer = null;
    }
    ctx.currentTimer = setTimeout(() => {
      ctx.add(ConnectionEvent.SendPing, null);
    }, HEARTBEAT_INTERVAL_MS);
  }
}

class MessageReceived implements IAction<ConnectionState, ConnectionEvent, ConnectionCtx, StateParam> {

  execute(evt: ConnectionEvent, ctx: ConnectionCtx, param?: MessageEnvelope) {
  }
}

class Connected extends BasicState<ConnectionState, ConnectionEvent, ConnectionCtx, StateParam> {

  events(): EventNode<ConnectionState, ConnectionEvent, ConnectionCtx, StateParam>[] {
    return [
      goToState(ConnectionEvent.HeartbeatFailed, ConnectionState.ConnectionFailed),
      goToState(ConnectionEvent.Reauthenticate, ConnectionState.UserAuthenicate),
      doEvt(ConnectionEvent.SendPing, new SendPing()),
      doEvt(ConnectionEvent.PongReceived, new PongReceived()),
      doEvt(ConnectionEvent.MessageReceived, new MessageReceived()),
      doEvt(ConnectionEvent.HeartbeatMissed, new HeartbeatMissed()),
      doEvt(ConnectionEvent.MessageEnvelopeReceived, new MessageEnvelopeReceived()),
      doEvt(ConnectionEvent.SendMessageEnvelope, new SendMessage())
    ];
  }

  entry(evt: ConnectionEvent, ctx: ConnectionCtx, param?: MessageEnvelope | UserLoginRs) {
    ctx.connectionRetryCount = 0;
    ctx.currentTimer = setTimeout(() => {
      ctx.add(ConnectionEvent.SendPing, null);
    }, HEARTBEAT_INTERVAL_MS);
  }

  state(): ConnectionState {
    return ConnectionState.Connected;
  }

}

class ConnectionFailed extends BasicState<ConnectionState, ConnectionEvent, ConnectionCtx, StateParam> {

  entry(evt: ConnectionEvent, ctx: ConnectionCtx, param?: MessageEnvelope | UserLoginRs) {
    if (ctx.currentTimer) {
      clearTimeout(ctx.currentTimer);
    }
    ctx.connectionRetryCount++;
    ctx.currentTimer = setTimeout(() => {
      ctx.add(ConnectionEvent.ReconnectTimeout, null);
    }, RECONNECT_RETRY);
  }

  exit(evt: ConnectionEvent, ctx: ConnectionCtx, param?: MessageEnvelope | UserLoginRs) {
    if (ctx.currentTimer) {
      clearTimeout(ctx.currentTimer);
      ctx.currentTimer = null;
    }
  }

  events(): EventNode<ConnectionState, ConnectionEvent, ConnectionCtx, StateParam>[] {
    return [
      goToState(ConnectionEvent.ReconnectTimeout, ConnectionState.Connecting)
    ];
  }

  state(): ConnectionState {
    return ConnectionState.ConnectionFailed;
  }
}

class UserAuthenticateAction implements IAction<ConnectionState, ConnectionEvent, ConnectionCtx, StateParam> {
  execute(evt: ConnectionEvent, ctx: ConnectionCtx, param?: StateParam) {
    const userLoginRq = (param as UserLoginRq);
    if (userLoginRq !== null) {
      ctx.hubConnection.send('auth', userLoginRq.username, userLoginRq.password);
    }
  }
}

class HeartbeatMissed implements IAction<ConnectionState, ConnectionEvent, ConnectionCtx, StateParam> {

  execute(evt: ConnectionEvent, ctx: ConnectionCtx, param?: StateParam) {
    if (ctx.currentTimer) {
      clearTimeout(ctx.currentTimer);
      ctx.currentTimer = null;
    }
    ctx.missedHeartbeats++;

    if (ctx.missedHeartbeats <= 3) {
      ctx.currentTimer = setTimeout(() => {
        ctx.add(ConnectionEvent.SendPing, null);
      }, HEARTBEAT_INTERVAL_MS);
    } else {
      ctx.add(ConnectionEvent.ConnectAttemptFailedEvt, null);
    }
  }
}

class MessageEnvelopeReceived implements IAction<ConnectionState, ConnectionEvent, ConnectionCtx, StateParam> {

  execute(evt: ConnectionEvent, ctx: ConnectionCtx, param?: StateParam) {
    const envelope = param as IMessageEnvelope;
    if (envelope) {
      ctx.envelopeSubject.next(envelope);
    }
  }

}

class UserLoginRs {

  constructor(
    public success: boolean,
    public connectionId: string
  ) {

  }
}

export class UserLoginRq {
  constructor(
    public username: string,
    public password: string
  ) {

  }
}

class SendMessage implements IAction<ConnectionState, ConnectionEvent, ConnectionCtx, StateParam> {
  execute(evt: ConnectionEvent, ctx: ConnectionCtx, param?: StateParam) {
    const envelope = param as IMessageEnvelope;
    if (envelope) {
      ctx.hubConnection.send('sendEnv', envelope);
    }
  }
}

@Injectable({
  providedIn: 'root'
})
export class SignalrConnectionService {

  private readonly connectionStateMachine = new StateMachine();
  private readonly connectionCtx = new ConnectionCtx();

  constructor(
    private ngZone: NgZone,
    private pendMessageService: PendingMessageService
  ) {
    this.connectionStateMachine
      .addState(new NotConnectedState())
      .addState(new Connecting())
      .addState(new UserAuthenticate())
      .addState(new Connected())
      .addState(new ConnectionFailed())
    ;
    this.connectionCtx.currentState = ConnectionState.NotConnected;
    this.connectionStateMachine.registerCtx(this.connectionCtx);
    this.pendMessageService.sendEnvelopeObservable.subscribe((env) => {
      this.sendEnv(env);
    });
    this.connectionCtx.envelopeSubject.subscribe(env => {
      pendMessageService.processEnvelope(env);
    });
  }

  /**
   * Used to start the connection process.
   */
  start() {
    this.connectionStateMachine.transition(
      this.connectionCtx,
      ConnectionEvent.ConnectEvt,
      null);
  }

  /**
   * Used to get the observable that has to do with authentication.
   */
  authRequestedObservable() {
    return this.connectionCtx.userAuthRequest.asObservable();
  }

  auth(userLogin: UserLoginRq) {
    this.connectionCtx.add(ConnectionEvent.UserAuthenticate, userLogin);
  }

  private sendEnv(envelope: IMessageEnvelope) {
    this.connectionCtx.add(
      ConnectionEvent.SendMessageEnvelope,
      envelope);
  }

  send<T>(message: Message<PayloadType, string>): Observable<IResultMonad<T>> {
    const subject = new Subject<IResultMonad<T>>();
    this.pendMessageService.addMessage(
      message,
      subject);
    return subject.asObservable();
  }
}
