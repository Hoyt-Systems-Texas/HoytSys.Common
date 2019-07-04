import { Injectable } from '@angular/core';
import {SignalrConnectionService, UserLoginRq} from './signalr-connection.service';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(
    private signalR: SignalrConnectionService
  ) {
    signalR.authRequestedObservable().subscribe(() => {
      this.handleAuth(signalR);
    });
  }

  handleAuth(signalRConn: SignalrConnectionService) {
    signalRConn.auth(new UserLoginRq(
      'test',
      'Password1'
    ));
  }
}
