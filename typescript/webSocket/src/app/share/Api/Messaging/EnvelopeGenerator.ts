import {Message} from './Message';
import {PayloadType} from './PayloadType';
import {IMessageEnvelope, MessageEnvelope} from './IMessageEnvelope';

const MAX_SIZE_BODY = 0x10000 - 1600; // Make user there is plenty of room.

/**
 * Used to generate an envelopes for a message.
 */
export class EnvelopeGenerator {


  /**
   * Used to create envelopes.
   * @param message The message to create the envelopes for.
   * @param handler The handler for the envelopes.
   */
  createEnvelopes(message: Message<PayloadType, string>, handler: (envelope: IMessageEnvelope) => void): number {
    if (message.body) {
      // Chop up the message.
      const total = this.calculateTotal(message);
      for (let i = 0 ; i < total ; i++) {
        handler(
          this.createEnvelope(
            message,
            i,
            total));
      }
      return total;
    } else {
      handler(new MessageEnvelope(
        0,
        0,
        0,
        0,
        message.messageType,
        message.payloadType,
        message.messageResultType,
        message.connectionId,
        message.correlationId,
        message.userId,
        null,
        message.body
      ));
      return 0;
    }
  }

  private calculateTotal(message: Message<PayloadType, string>): number {
    let total = Math.floor(message.body.length / MAX_SIZE_BODY);
    if (message.body.length % MAX_SIZE_BODY > 0) {
      total++;
    }
    return total;
  }

  /**
   *  Used to create an envelope fragment.
   * @param message The message to create the fragment from.
   * @param handler The handler for the message fragment.
   */
  createFragment(message: Message<PayloadType, string>, pos: number, handler: (envelope: IMessageEnvelope) => void) {
    handler(this.createEnvelope(
      message,
      pos,
      this.calculateTotal(message)
    ));
  }

  private createEnvelope(message: Message<PayloadType, string>, i: number, total: number) {
    const bodyLength = i * MAX_SIZE_BODY;
    const length = Math.min(message.body.length - bodyLength, MAX_SIZE_BODY);
    return new MessageEnvelope(
      0,
      i,
      total,
      message.body.length,
      message.messageType,
      message.payloadType,
      0,
      message.connectionId,
      message.correlationId,
      null,
      null,
      message.body.substr(
        i * MAX_SIZE_BODY,
        length));
  }
}
