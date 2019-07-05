import { Injectable } from '@angular/core';
import {SignalrConnectionService} from './signalr-connection.service';
import {Observable} from 'rxjs';
import {IResultMonad} from '../Api/Monad/ResultMonad';
import {PayloadType} from '../Api/Messaging/PayloadType';

@Injectable({
  providedIn: 'root'
})
export class TestMessageService {

  constructor(
    private signalRService: SignalrConnectionService
  ) { }

  sendTest(): Observable<IResultMonad<any>> {
    return this.signalRService.send(PayloadType.SendTest, 'hi');
  }
}
