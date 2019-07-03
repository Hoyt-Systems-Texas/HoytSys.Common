import {RingBuffer} from './RingBuffer';

export class SkipQueueRingBuffer<T> {

  buffer: RingBuffer<SkipQueueNode<T>>;
  reader = 0;
  readerSkip = 0;
  writer = 0;

  constructor(
    public readonly length: number
  ) {
    this.buffer = new RingBuffer<SkipQueueNode<T>>(length);
    for (let i = 0; i < length; i++) {
      const node = new SkipQueueNode<T>();
      node.isSet = false;
      node.position = i;
      this.buffer.set(i, node);
    }
  }

  offer(value: T): boolean {
    if (this.isFull()) {
      return false;
    }
    const node = this.buffer.get(this.writer);
    node.position = this.writer;
    node.isSet = true;
    node.value = value;
    this.writer++;
    return true;
  }

  peek(): [boolean, T] {
    if (this.reader === this.writer) {
      return [false, null];
    } else {
      return [true, this.buffer.get(this.reader).value];
    }
  }

  /**
   * Used to get the next reader value.
   */
  poll(): [boolean, T] {
    if (this.readerSkip === this.writer) {
      return [false, null];
    } else {
      const node = this.buffer.get(this.readerSkip);
      const value = node.value;
      node.isSet = false;
      node.value = null;
      if (this.readerSkip === this.reader) {
        this.readerSkip++;
        this.reader++;
      }
      this.goToNextNode();
      return [true, value];
    }
  }

  private goToNextNode() {
    while (this.reader < this.writer
    && !this.buffer.get(this.reader).isSet) {
      this.reader++;
    }
    while (this.readerSkip < this.writer
    && !this.buffer.get(this.readerSkip).isSet) {
      this.readerSkip++;
    }
  }

  skip() {
    if (this.reader <= this.writer) {
      this.readerSkip++;
    }
  }

  /**
   * Resets to the first non skip value.
   */
  resetToFirst() {
    this.readerSkip = this.reader;
  }

  /**
   * return true if the queue is full.
   */
  isFull() {
    return this.writer - this.reader >= this.length;
  }

}

class SkipQueueNode<T> {
  position: number;
  value: T;
  isSet: boolean;
}
