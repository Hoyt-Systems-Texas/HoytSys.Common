import {SkipQueueRingBuffer} from '../DataStructures/SkipQueueRingBuffer';
import {BoundElementProperty} from '@angular/compiler';

export class BaseContext<TState, TEvent, TParam> {

  private eventQueue = new SkipQueueRingBuffer<EventNode<TEvent, TParam>>(64);

  currentState: TState;

  nextEvent(): [boolean, EventNode<TEvent, TParam>] {
    const node = this.eventQueue.peek();
    return node;
  }

  accept(): [boolean, EventNode<TEvent, TParam>] {
    return this.eventQueue.poll();
  }

  skipNext() {
    this.eventQueue.skip();
  }

  add(event: TEvent, param: TParam): boolean {
    return this.eventQueue.offer(new EventNode<TEvent, TParam>(event, param));
  }

}

export class EventNode<TEvent, TParam> {

  constructor(
    public event: TEvent,
    public param: TParam
  ) {

  }
}
