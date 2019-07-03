export class BaseContext<TState, TEvent, TParam> {

  currentState: TState;

  eventQueue: TEvent[];
}
