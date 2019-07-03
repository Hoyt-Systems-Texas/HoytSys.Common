import {EventAction, EventNode, IState} from './IState';

export class StateMachine<TState, TEvent, TCtx, TParam> {

  stateLookup = new Map<TState, IState<TState, TEvent, TCtx, TParam>>();

  /**
   * Used to execute the transition.
   * @param event The event
   * @param param The parameter associated with the event.
   */
  transition(event: TEvent, param: TParam) {

  }

  /**
   * Used to add a state to the state machine.
   * @param state The state to add to the state machine.
   */
  addState(state: IState<TState, TEvent, TCtx, TParam>) {

  }

}

class StateNode<TState, TEvent, TCtx, TParam> {

  eventActLookup = new Map<TEvent, EventNode<TState, TEvent, TCtx, TParam>>();

  constructor(
    public state: IState<TState, TEvent, TCtx, TParam>
  ) {
    this.state.events().forEach(eventNode => {
      if (!this.eventActLookup.has(eventNode.event)) {
        this.eventActLookup.set(eventNode.event, eventNode);
      } else {
        throw Error(`Already registered event ${eventNode.event}`);
      }
    });
  }

  handle(evt: TEvent, ctx: TCtx, param: TParam) {
    if (this.eventActLookup.has(evt)) {
      const node = this.eventActLookup.get(evt);
      switch (node.eventAction) {
        case EventAction.Defer:
          break;

        case EventAction.Do:
          break;

        case EventAction.Ignore:
          break;

        case EventAction.Goto:
          break;

      }
    }
  }
}
