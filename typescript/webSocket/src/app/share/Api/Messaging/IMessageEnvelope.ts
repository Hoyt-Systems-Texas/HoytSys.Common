import {MessageType} from './MessageType';
import {PayloadType} from './PayloadType';
import {MessageResultType} from './MessageResultType';

export interface IMessageEnvelope {
  requestId: number;
  number: number;
  total: number;
  totalBodyLength: number;
  messageType: MessageType;
  payloadType: PayloadType;
  messageResultType: MessageResultType;
  connectionId: string;
  correlationId: number;
  userId: string;
  toConnectionId: string;
  body: string;
}

export class MessageEnvelope implements IMessageEnvelope {

  constructor(
    public requestId: number,
    // tslint:disable-next-line:variable-name
    public number: number,
    public total: number,
    public totalBodyLength: number,
    public messageType: MessageType,
    public payloadType: PayloadType,
    public messageResultType: MessageResultType,
    public connectionId: string,
    public correlationId: number,
    public userId: string,
    public toConnectionId: string,
    public body: string) {

  }
}

