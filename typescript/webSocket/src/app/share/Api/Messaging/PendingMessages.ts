import {Message} from './Message';
import {PayloadType} from './PayloadType';
import {IMessageEnvelope, MessageEnvelope} from './IMessageEnvelope';
import {IncomingMessageBuilder} from './IncomingMessageBuilder';
import {EnvelopeGenerator} from './EnvelopeGenerator';
import {Observable, Subject} from 'rxjs';
import {IResultMonad} from '../Monad/ResultMonad';
import {MessageType} from './MessageType';
import Timer = NodeJS.Timer;

const messageBuilder = new IncomingMessageBuilder();
const envelopeGenerator = new EnvelopeGenerator();
const STATUS_TIMOUT_MS = 2000;
const FRAGMENT_TIMEOUT_MS = 500;

enum MessageState {
  New = 0,
  Sent,
  ReceivedStatus, // Has envelope
  Receiving, // Has envelope
  Timeout,
  ResponseReceived // Has a message
}

enum MessageEvent {
  Sending = 0,
  StatusUpdateTimeout,
  RequestTimeout,
  StatusUpdate,
  FragmentTimeout,
  ReceivingResponse,
  ResponseCompleted
}

export class PendingMessages {

  private messages = new Map<string, PendingStateMachine>();
  private sendEnvelopeSubject = new Subject<IMessageEnvelope>();
  private eventSubject = new Subject<Message<PayloadType, string>>();

  public add<T>(message: Message<PayloadType, string>): Observable<IResultMonad<T>> {
    const key = this.createKeyMsg(message);
    if (!this.messages.has(key)) {
      const messageMachine = new MessageStateMachine<T>(message);
      this.messages.set(key, messageMachine);
      messageMachine.envelopeObservable.subscribe(env => this.sendEnvelopeSubject.next(env));
      messageMachine.changeState(MessageEvent.Sending);
      return messageMachine.responseObservable;
    }
    return null;
  }

  /**
   * Used to create a key.
   * @param envelope The envelope to create the key for.
   */
  private createKeyEnv(envelope: IMessageEnvelope): string {
    return envelope.connectionId + '##' + envelope.correlationId;
  }

  private createKeyMsg(message: Message<PayloadType, string>) {
    return message.connectionId + '##' + message.correlationId;
  }

  /**
   * The observable that has the envelopes we want to send to the backend.
   */
  get sendEnvelope(): Observable<IMessageEnvelope> {
    return this.sendEnvelopeSubject.asObservable();
  }

  get events(): Observable<Message<PayloadType, string>> {
    return this.eventSubject.asObservable();
  }
}

interface PendingStateMachine extends Destroy {
  changeState(evt: MessageEvent);
}

class MessageStateMachine<T> {

  private isEnvelope: boolean;
  private currentState: MessageState = MessageState.New;
  private envelopeSubject = new Subject<IMessageEnvelope>();
  private responseSubject = new Subject<IResultMonad<T>>();
  private currentTimer: Timer;

  constructor(
    public readonly message: Message<PayloadType, string>
  ) {

  }

  changeState(evt: MessageEvent, envelope?: IMessageEnvelope | Message<PayloadType, string>) {
    switch (evt) {
      case MessageEvent.Sending:
        if (this.currentState === MessageState.New
          || this.currentState === MessageState.Sent) {
          this.currentState = MessageState.Sent;
          this.send(evt);
        }
        break;

      case MessageEvent.StatusUpdateTimeout:
        break;

      case MessageEvent.RequestTimeout:
        break;

      case MessageEvent.StatusUpdate:
        break;

      case MessageEvent.FragmentTimeout:
        break;

      case MessageEvent.ReceivingResponse:
        break;

      case MessageEvent.ResponseCompleted:
        break;
    }
  }

  get envelopeObservable(): Observable<IMessageEnvelope> {
    return this.envelopeSubject.asObservable();
  }

  get responseObservable(): Observable<IResultMonad<T>> {
    return this.responseSubject.asObservable();
  }

  private send(evt: MessageEvent) {
    envelopeGenerator.createEnvelopes(this.message, this.envelopeSubject.next);
    if (this.currentTimer) {
      clearTimeout(this.currentTimer);
      this.currentTimer = null;
    }
    this.currentTimer = setTimeout(() => {
      this.changeState(MessageEvent.StatusUpdateTimeout);
    }, STATUS_TIMOUT_MS);
  }

  private receivedStatus(evt: MessageEvent, message: IMessageEnvelope) {
    if (evt === MessageEvent.StatusUpdateTimeout) {
      this.envelopeSubject.next(new MessageEnvelope(
        0,
        0,
        0,
        0,
        MessageType.Status,
        message.payloadType,
        null,
        message.connectionId,
        message.correlationId,
        null,
        null,
        null
      ));
      if (this.currentTimer) {
        clearTimeout(this.currentTimer);
      }
      this.currentTimer = setTimeout(() => {
        this.changeState(MessageEvent.StatusUpdateTimeout);
      }, STATUS_TIMOUT_MS);
    }
  }

  private receiving(evt: MessageEvent, message: IMessageEnvelope) {
    if (evt === MessageEvent.ReceivingResponse) {
      this.currentState = MessageState.Receiving;
      const result = messageBuilder.add(message);
      if (result[0]) {
        this.changeState(MessageEvent.ReceivingResponse, result[1]);
      }
      clearTimeout(this.currentTimer);
      this.currentTimer = setTimeout(() => {
        this.changeState(MessageEvent.RequestTimeout);
      }, FRAGMENT_TIMEOUT_MS);
    } else if (evt === MessageEvent.RequestTimeout) {
      // TODO look for missing fragments.
    }
  }

  private timeout(evt: MessageEvent) {
  }

  private responseReceived(evt: MessageEvent, message: Message<PayloadType, string>) {

  }

  /**
   * Used to release any memory/observables.
   */
  destroy() {
    this.envelopeSubject.complete();
    this.responseSubject.complete();
    clearTimeout(this.currentTimer);
  }
}
