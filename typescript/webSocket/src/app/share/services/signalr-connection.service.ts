import * as signalR from '@aspnet/signalr';
import { Injectable } from '@angular/core';
import {environment} from '../../../environments/environment';
import {HubConnection} from '@aspnet/signalr';

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

  constructor() { }

  setupConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(environment.signalRConnection)
      .configureLogging(signalR.LogLevel.Information)
      .build();
  }

  /**
   * Used to change the state.
   * @param evt The event.
   */
  changeState(evt: ConnectionEvent) {

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

        }
        break;

      case ConnectionEvent.PongReceived:
        if (this.currentState === ConnectionState.Connected) {
          this.resetHeartbeatPing();
        }
        break;

      case ConnectionEvent.MessageReceived:
        if (this.currentState === ConnectionState.Connected) {
          this.resetHeartbeatPing();
        }
        break;

    }
  }

  private handleConnectionFailed() {
    this.currentState = ConnectionState.ConnectionFailed;
    // TODO start timer to reconnect
  }

  private sendPing() {

  }

  private handlePong() {
    this.lastReceivedDate = new Date();
  }

  private resetHeartbeatPing() {

  }
}