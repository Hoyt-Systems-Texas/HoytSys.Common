import * as signalR from '@aspnet/signalr';
import { Injectable } from '@angular/core';
import {environment} from '../../../environments/environment';
import {HubConnection} from '@aspnet/signalr';

export enum ConnectionState {
  Disconnected = 0,
  Connecting = 1,
  Connected = 2,
  Reconnect = 3,
  Reconnecting = 4,
  ReconnectFailed = 5
}

@Injectable({
  providedIn: 'root'
})
export class SignalrConnectionService {

  private hubConnection: HubConnection;

  constructor() { }

  setupConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(environment.signalRConnection)
      .configureLogging(signalR.LogLevel.Information)
      .build();
  }

}
