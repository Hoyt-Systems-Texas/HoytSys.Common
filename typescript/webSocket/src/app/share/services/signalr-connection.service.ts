import * as signalR from '@aspnet/signalr';
import {Injectable, NgZone} from '@angular/core';
import {environment} from '../../../environments/environment';
import {HubConnection} from '@aspnet/signalr';
import {Subject} from 'rxjs';
import {MessageType} from '../Api/Messaging/MessageType';
import {PayloadType} from '../Api/Messaging/PayloadType';
import {MessageResultType} from '../Api/Messaging/MessageResultType';
import {IMessageEnvelope} from '../Api/Messaging/IMessageEnvelope';

const HEARTBEAT_INTERVAL_MS = 5000;

export enum ConnectionState {
  NotConnected = 0,
  Connecting = 1,
  Connected = 2,
  ConnectionFailed = 3,

}

export enum ConnectionEvent {
  ConnectEvt = 0,
  ConnectSuccessfulEvt = 1,
  ConnectAttemptFailedEvt = 2,
  HeartbeatFailed = 3,
  SendPing = 4,
  PongReceived = 5,
  MessageReceived = 6
}

class UserLoginRs {

  constructor(
    public success: boolean,
    public connectionId: string
  ) {

  }
}

@Injectable({
  providedIn: 'root'
})
export class SignalrConnectionService {

  private hubConnection: HubConnection;
  private currentState: ConnectionState = ConnectionState.NotConnected;
  private connectRetryCount = 0;
  private lastReceivedDate: Date;
  private heartbeatSent: Date;
  private heartbeatTotal = 0;
  private heartbeatTotalSent = 0;
  private authResponseSubject = new Subject<UserLoginRs>();
  private messageReceivedSubject = new Subject<IMessageEnvelope>();
  private pongSubject = new Subject();

  constructor(
    ngZone: NgZone
  ) { }

  setupConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(environment.signalRConnection)
      .configureLogging(signalR.LogLevel.Information)
      .build();

    this.hubConnection.start().catch(err => {
      console.error(err);
    });

    this.hubConnection.on('authResponse', (success: boolean, connection: string) => {
      this.authResponseSubject.next(new UserLoginRs(success, connection));
    });
    this.hubConnection.on('received', (envelope) => {
      this.changeState(ConnectionEvent.MessageReceived, envelope);
    });
    this.hubConnection.on('pong', () => {
      this.changeState(ConnectionEvent.PongReceived);
    });
  }

  /**
   * Used to change the state.
   * @param evt The event.
   */
  changeState(evt: ConnectionEvent, data?: IMessageEnvelope) {

    switch (evt) {
      case ConnectionEvent.ConnectEvt:
        if (this.currentState === ConnectionState.NotConnected
        || this.currentState === ConnectionState.ConnectionFailed) {
        }
        break;

      case ConnectionEvent.ConnectSuccessfulEvt:
        if (this.currentState === ConnectionState.Connecting) {
          this.currentState = ConnectionState.Connected;
          this.connectRetryCount = 0;
        }
        break;

      case ConnectionEvent.ConnectAttemptFailedEvt:
        if (this.currentState === ConnectionState.Connecting) {
          this.handleConnectionFailed();
        }
        break;

      case ConnectionEvent.HeartbeatFailed:
        if (this.currentState === ConnectionState.Connected) {
          this.handleConnectionFailed();
        }
        break;

      case ConnectionEvent.SendPing:
        if (this.currentState === ConnectionState.Connected) {
          this.sendPing();
        }
        break;

      case ConnectionEvent.PongReceived:
        if (this.currentState === ConnectionState.Connected) {
          this.resetHeartbeatPing();
          this.handlePong();
        }
        break;

      case ConnectionEvent.MessageReceived:
        if (this.currentState === ConnectionState.Connected) {
          this.resetHeartbeatPing();
          if (data) {
            this.handleReceived(data);
          }
        }
        break;

    }
  }

  private handleReceived(envelope: IMessageEnvelope) {

  }

  private handleConnectionFailed() {
    this.currentState = ConnectionState.ConnectionFailed;
    // TODO start timer to reconnect
  }

  private sendPing() {
    this.hubConnection.send('ping');
  }

  private handlePong() {
    this.lastReceivedDate = new Date();
  }

  private resetHeartbeatPing() {

  }
}
