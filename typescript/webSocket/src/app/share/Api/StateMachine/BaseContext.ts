import {SkipQueueRingBuffer} from '../DataStructures/SkipQueueRingBuffer';
import {BoundElementProperty} from '@angular/compiler';
import {Observable, Subject} from 'rxjs';

export class BaseContext<TState, TEvent, TParam> implements Destroy {

  private eventQueue = new SkipQueueRingBuffer<EventNode<TEvent, TParam>>(64);
  private eventAdded = new Subject();

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
    const success = this.eventQueue.offer(new EventNode<TEvent, TParam>(event, param));
    this.eventAdded.next();
    return success;
  }

  destroy() {
    this.eventAdded.complete();
  }

  eventAddedObservable(): Observable<any> {
    return this.eventAdded.asObservable();
  }
}

export class EventNode<TEvent, TParam> {

  constructor(
    public event: TEvent,
    public param: TParam
  ) {

  }
}
