import {MessageResultType} from './MessageResultType';
import {MessageType} from './MessageType';
import {BodyReconstructor} from './BodyReconstructor';
import {Message} from './Message';
import {PayloadType} from './PayloadType';

export class MessageBuilder {
  public lastTouched: Date;
  private bodyReconstructor: BodyReconstructor;
  constructor(
    public readonly requestId: number,
    public readonly correlationId: number,
    public readonly connectionId: string,
    public readonly total: number,
    public readonly messageType: MessageType,
    public readonly messageResultType: MessageResultType,
    public readonly payloadType: PayloadType,
    public readonly userId: string
  ) {
    this.bodyReconstructor = new BodyReconstructor(total);
  }

  get isCompleted(): boolean {
    return this.bodyReconstructor.isCompleted();
  }

  append(position: number, body: string) {
    this.bodyReconstructor.append(position, body);
    this.lastTouched = new Date();
  }

  get message(): Message<PayloadType, string> {
    return new Message<PayloadType, string>(
      this.requestId,
      this.connectionId,
      this.correlationId,
      this.messageType,
      this.messageResultType,
      this.bodyReconstructor.body,
      this.payloadType,
      this.userId
    );
  }
}
