import {EventAction, EventNode, IState} from './IState';
import {BaseContext} from './BaseContext';

/**
 * A simple state machine.
 */
export class StateMachine<TState, TEvent, TCtx extends BaseContext<TState, TEvent, TParam>, TParam> {

  stateLookup = new Map<TState, StateNode<TState, TEvent, TCtx, TParam>>();

  /**
   * Used to execute the transition.
   * @param ctx The current context.
   * @param event The event
   * @param param The parameter associated with the event.
   */
  transition(
    ctx: TCtx,
    event: TEvent,
    param: TParam) {
    ctx.add(event, param);
    this.handleEvent(ctx);
  }

  /**
   * Used to add a state to the state machine.
   * @param state The state to add to the state machine.
   */
  addState(state: IState<TState, TEvent, TCtx, TParam>): StateMachine<TState, TEvent, TCtx, TParam> {
    if (this.stateLookup.has(state.state())) {
      throw new Error(`State has already been registered ${state.state()}`);
    } else {
      this.stateLookup.set(state.state(), new StateNode<TState, TEvent, TCtx, TParam>(state));
    }
    return this;
  }

  /**
   * used to handle to the next event.
   * @param ctx The context for the state machine.
   */
  private handleEvent(
    ctx: TCtx
  ) {
    const state = this.stateLookup.get(ctx.currentState);
    const event = ctx.nextEvent();
    if (event[0]) {
      const eventNode = event[1];
      if (state.eventActLookup.has(eventNode.event)) {
        const actionNode = state.eventActLookup.get(eventNode.event);
        switch (actionNode.eventAction) {
          case EventAction.Goto:
            if (this.stateLookup.has(actionNode.state)) {
              state.state.exit(eventNode.event, ctx, eventNode.param);
              ctx.currentState = actionNode.state;
              const stateNext = this.stateLookup.get(ctx.currentState);
              stateNext.state.entry(
                eventNode.event,
                ctx,
                eventNode.param);
            } else {

            }
            ctx.accept();
            this.handleEvent(ctx);
            break;

          case EventAction.Do:
            actionNode.action.execute(
              eventNode.event,
              ctx,
              eventNode.param);
            ctx.accept();
            this.handleEvent(ctx);
            break;

          case EventAction.Defer:
            ctx.skipNext();
            this.handleEvent(ctx);
            break;

          case EventAction.Ignore:
            ctx.accept();
            this.handleEvent(ctx);
            break;
        }
      } else {
        ctx.accept();
        console.log(`Unable to find event for ${eventNode.event}`);
      }
    }
  }

}

class StateNode<TState, TEvent, TCtx extends BaseContext<TState, TEvent, TParam>, TParam> {

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
}
