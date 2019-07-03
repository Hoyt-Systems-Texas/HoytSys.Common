import {MessageType} from './MessageType';
import {MessageResultType} from './MessageResultType';

export class Message<TPayloadType, TBody> {

  constructor(
    public requestId: number,
    public connectionId: string,
    public correlationId: number,
    public messageType: MessageType,
    public messageResultType: MessageResultType,
    public body: TBody,
    public payloadType: TPayloadType,
    public userId: string) {

  }
}
