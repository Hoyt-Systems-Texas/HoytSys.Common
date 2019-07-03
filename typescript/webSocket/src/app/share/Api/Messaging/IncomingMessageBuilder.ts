import {MessageBuilder} from './MessageBuilder';
import {IMessageEnvelope} from './IMessageEnvelope';
import {Message} from './Message';
import {PayloadType} from './PayloadType';

/**
 * Used to process incoming envelopes.
 */
export class IncomingMessageBuilder {

  pendingMessages = new Map<number, MessageBuilder>();

  /**
   * Used to add an envelope to the connection.
   * @param envelope The envelope to add to the collection.
   * @returns a tuple with the first value indicating if we received all of the blocks and the second value is the completed message if we have.
   */
  public add(
    envelope: IMessageEnvelope): [boolean, Message<PayloadType, string>] {
    if (!this.pendingMessages.has(envelope.requestId)) {
      this.pendingMessages.set(envelope.requestId, new MessageBuilder(
        envelope.requestId,
        envelope.correlationId,
        envelope.connectionId,
        envelope.total,
        envelope.messageType,
        envelope.messageResultType,
        envelope.payloadType,
        envelope.userId
      ));
    }
    const builder = this.pendingMessages.get(envelope.requestId);
    if (builder.isCompleted) {
      this.pendingMessages.delete(envelope.requestId);
      return [true, builder.message];
    }
    return [false, null];
  }
}
