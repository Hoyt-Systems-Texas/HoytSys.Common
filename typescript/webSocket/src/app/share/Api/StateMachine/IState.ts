import {BaseContext} from './BaseContext';

export interface IState<TState, TEvent, TCtx extends BaseContext<TState, TEvent, TParam>, TParam> {

  state(): TState;

  events(): EventNode<TState, TEvent, TCtx, TParam>[];

  entry(evt: TEvent, ctx: TCtx, param?: TParam);

  exit(evt: TEvent, ctx: TCtx, param?: TParam);
}

export abstract class BasicState<TState, TEvent, TCtx
  extends BaseContext<TState, TEvent, TParam>, TParam>
  implements IState<TState, TEvent, TCtx, TParam> {

  entry(evt: TEvent, ctx: TCtx, param?: TParam) {
  }

  abstract events(): EventNode<TState, TEvent, TCtx, TParam>[];

  exit(evt: TEvent, ctx: TCtx, param?: TParam) {
  }

  abstract state(): TState;

}


export enum EventAction {
  Do = 0,
  Goto = 1,
  Defer = 2,
  Ignore = 3
}

export class EventNode<TState, TEvent, TCtx, TParam> {

  constructor(
    public event: TEvent,
    public eventAction: EventAction,
    public state?: TState,
    public action?: IAction<TState, TEvent, TCtx, TParam>
  ) {

  }
}

export function goToState<TState, TEvent, TCtx, TParam>(
  evt: TEvent,
  state: TState
) {
  return new EventNode(evt, EventAction.Goto, state);
}

export function deferEvt<TState, TEvent, TCtx, TParam>(
  evt: TEvent) {
  return new EventNode(evt, EventAction.Defer);
}

export function doEvt<TState, TEvent, TCtx, TParam>(
  evt: TEvent,
  action: IAction<TState, TEvent, TCtx, TParam>) {
  return new EventNode(evt, EventAction.Do, null, action);
}

export function ignoreEvt<TState, TEvent, TCtx, TParam>(
  evt: TEvent) {
  return new EventNode(evt, EventAction.Ignore, null, null);
}
