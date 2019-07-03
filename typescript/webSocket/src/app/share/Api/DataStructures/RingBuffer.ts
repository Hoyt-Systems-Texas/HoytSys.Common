/**
 * Very basic ring buffer implementation.
 */
export class RingBuffer<T> {

  private buffer: T[] = [];

  constructor(
    public length: number
  ) {

  }

  set(pos: number, value: T) {
    this.buffer[this.calcPos(pos)] = value;
  }

  get(pos: number): T {
    return this.buffer[this.calcPos(pos)];
  }

  calcPos(pos: number) {
    return pos % this.length;
  }
}

