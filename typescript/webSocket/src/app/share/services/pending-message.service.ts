import {Injectable} from '@angular/core';
import {IMessageEnvelope, MessageEnvelope} from '../Api/Messaging/IMessageEnvelope';
import {Message} from '../Api/Messaging/Message';
import {PayloadType} from '../Api/Messaging/PayloadType';
import {BaseContext} from '../Api/StateMachine/BaseContext';
import {Observable, Subject} from 'rxjs';
import {BasicState, doEvt, EventNode, goToState} from '../Api/StateMachine/IState';
import {IncomingMessageBuilder} from '../Api/Messaging/IncomingMessageBuilder';
import {EnvelopeGenerator} from '../Api/Messaging/EnvelopeGenerator';
import {IResultMonad, ResultError} from '../Api/Monad/ResultMonad';
import {StateMachine} from '../Api/StateMachine/StateMachine';
import {MessageType} from '../Api/Messaging/MessageType';
import Timer = NodeJS.Timer;

const messageBuilder = new IncomingMessageBuilder();
const envelopeGenerator = new EnvelopeGenerator();
const STATUS_TIMOUT_MS = 2000;
const FRAGMENT_TIMEOUT_MS = 500;
const REQUEST_TIMEOUT_MS = 1000 * 60 * 3;

enum MessageState {
  New = 0,
  Sent,
  StatusReceived, // Has envelope
  Receiving, // Envelopes being received.
  Timeout,
  ResponseReceived, // Has a message
  EventReceived
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

type PendingMessageParam = MessageEnvelope | Message<PayloadType, string>;

class PendingMessageCtx extends BaseContext<MessageState, MessageEvent, PendingMessageParam> {

  requestTimeout: Timer;
  currentTimer: Timer;
  message: Message<PayloadType, string>;
  public completed = new Subject();

  constructor(
    public readonly envelopeSubject: Subject<MessageEnvelope>,
    public readonly resultSubject: Subject<any>
  ) {
    super();
  }

  get completedObservable(): Observable<any> {
    return this.completed.asObservable();
  }

  destroy() {
    super.destroy();
    this.completed.next();
    this.completed.complete();
  }
}

class NewState extends BasicState<MessageState, MessageEvent, PendingMessageCtx, PendingMessageParam> {
  exit(evt: MessageEvent, ctx: PendingMessageCtx, param?: PendingMessageParam) {
    ctx.requestTimeout = setTimeout(() => {
      ctx.add(MessageEvent.RequestTimeout, null);
    }, REQUEST_TIMEOUT_MS);
  }

  events(): EventNode<MessageState, MessageEvent, PendingMessageCtx, PendingMessageParam>[] {
    return [
      goToState(MessageEvent.Sending, MessageState.Sent)
    ];
  }

  state(): MessageState {
    return MessageState.New;
  }
}

class SentState extends BasicState<MessageState, MessageEvent, PendingMessageCtx, PendingMessageParam> {

  entry(evt: MessageEvent, ctx: PendingMessageCtx, param?: PendingMessageParam) {
    if (ctx.currentTimer) {
      clearTimeout(ctx.currentTimer);
    }
    envelopeGenerator.createEnvelopes(
      ctx.message,
      (env) => {
        ctx.envelopeSubject.next(env);
      });
    ctx.currentTimer = setTimeout(() => {
      ctx.add(MessageEvent.StatusUpdateTimeout, null);
    }, STATUS_TIMOUT_MS);
  }

  events(): EventNode<MessageState, MessageEvent, PendingMessageCtx, PendingMessageParam>[] {
    return [
      goToState(MessageEvent.StatusUpdateTimeout, MessageState.Sent),
      goToState(MessageEvent.StatusUpdate, MessageState.StatusReceived),
      goToState(MessageEvent.RequestTimeout, MessageState.Timeout)
    ];
  }

  state(): MessageState {
    return MessageState.Sent;
  }
}

class StatusReceived extends BasicState<MessageState, MessageEvent, PendingMessageCtx, PendingMessageParam> {

  entry(evt: MessageEvent, ctx: PendingMessageCtx, param?: PendingMessageParam) {
    if (ctx.currentTimer) {
      clearTimeout(ctx.currentTimer);
    }
    ctx.currentTimer = null;
  }

  events(): EventNode<MessageState, MessageEvent, PendingMessageCtx, MessageEnvelope | Message<PayloadType, string>>[] {
    return [
      goToState(MessageEvent.RequestTimeout, MessageState.Timeout),
      goToState(MessageEvent.ReceivingResponse, MessageState.Receiving),
      goToState(MessageEvent.StatusUpdate, MessageState.StatusReceived)
    ];
  }

  state(): MessageState {
    return MessageState.StatusReceived;
  }
}

class Receiving extends BasicState<MessageState, MessageEvent, PendingMessageCtx, PendingMessageParam> {

  entry(evt: MessageEvent, ctx: PendingMessageCtx, param?: PendingMessageParam) {
    const envelope = param as IMessageEnvelope;
    const result = messageBuilder.add(envelope);
    if (result[0]) {
    }
  }

  events(): EventNode<MessageState, MessageEvent, PendingMessageCtx, MessageEnvelope | Message<PayloadType, string>>[] {
    return [
      goToState(MessageEvent.RequestTimeout, MessageState.Timeout),
      goToState(MessageEvent.ResponseCompleted, MessageState.ResponseReceived),
      doEvt(MessageEvent.FragmentTimeout, new FragmentTimeout()),
      goToState(MessageEvent.ReceivingResponse, MessageState.Receiving)
    ];
  }

  state(): MessageState {
    return MessageState.Receiving;
  }
}

class FragmentTimeout implements IAction<MessageState, MessageEvent, PendingMessageCtx, PendingMessageParam> {
  execute(evt: MessageEvent, ctx: PendingMessageCtx, param?: PendingMessageParam) {
  }
}

class EventReceived extends BasicState<MessageState, MessageEvent, PendingMessageCtx, PendingMessageParam> {
  events(): EventNode<MessageState, MessageEvent, PendingMessageCtx, PendingMessageParam>[] {
    return [
      goToState(MessageEvent.ReceivingResponse, MessageState.Receiving)
    ];
  }

  state(): MessageState {
    return MessageState.EventReceived;
  }
}

class TimedOut extends BasicState<MessageState, MessageEvent, PendingMessageCtx, PendingMessageParam> {

  entry(evt: MessageEvent, ctx: PendingMessageCtx, param?: PendingMessageParam) {
    ctx.destroy();
    ctx.resultSubject.next(new ResultError(['Request has timed out']));
    ctx.resultSubject.complete();
  }

  events(): EventNode<MessageState, MessageEvent, PendingMessageCtx, PendingMessageParam>[] {
    return [];
  }

  state(): MessageState {
    return MessageState.Timeout;
  }
}

class ResponseReceived extends BasicState<MessageState, MessageEvent, PendingMessageCtx, PendingMessageParam> {

  entry(evt: MessageEvent, ctx: PendingMessageCtx, param?: PendingMessageParam) {
    ctx.destroy();
  }

  events(): EventNode<MessageState, MessageEvent, PendingMessageCtx, MessageEnvelope | Message<PayloadType, string>>[] {
    return [];
  }

  state(): MessageState {
    return MessageState.ResponseReceived;
  }
}

@Injectable({
  providedIn: 'root'
})
export class PendingMessageService {

  private sendEnvelopeSubject = new Subject<IMessageEnvelope>();
  private messageStateMachine: StateMachine<MessageState, MessageEvent, PendingMessageCtx, PendingMessageParam>;
  private pendingMessageMap = new Map<string, PendingMessageCtx>();

  constructor() {
    this.messageStateMachine = new StateMachine();
    this.messageStateMachine
      .addState(new NewState())
      .addState(new SentState())
      .addState(new EventReceived())
      .addState(new StatusReceived())
      .addState(new Receiving())
      .addState(new TimedOut())
      .addState(new ResponseReceived())
    ;

  }

  addMessage<T>(
    message: Message<PayloadType, string>,
    resultSubject: Subject<IResultMonad<T>>) {
    const ctx = new PendingMessageCtx(
      this.sendEnvelopeSubject,
      resultSubject);
    ctx.currentState = MessageState.New;
    const key = this.createKeyMsg(message);
    if (!this.pendingMessageMap.has(key)) {
      this.pendingMessageMap.set(key, ctx);
      ctx.completedObservable.subscribe(() => {
        this.pendingMessageMap.delete(key);
      });
    }
  }

  processEnvelope(
    env: MessageEnvelope) {
    const key = this.createKeyEnv(env);
    if (this.pendingMessageMap.has(key)) {
      const ctx = this.pendingMessageMap.get(key);
      this.messageStateMachine.transition(
        ctx,
        MessageEvent.ReceivingResponse,
        env);
    } else {
      if (env.messageType === MessageType.Event) {
        const ctx = new PendingMessageCtx(
          this.sendEnvelopeSubject,
          null
        );
        ctx.currentState = MessageState.EventReceived;
        this.pendingMessageMap.set(key, ctx);
        ctx.completedObservable.subscribe(() => {
          this.pendingMessageMap.delete(key);
        });
      }
    }
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

  get sendEnvelopeObservable(): Observable<MessageEnvelope> {
    return this.sendEnvelopeSubject.asObservable();
  }

}
