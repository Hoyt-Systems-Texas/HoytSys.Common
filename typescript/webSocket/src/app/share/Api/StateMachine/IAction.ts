interface IAction<TState, TEvent, TCtx, TParam> {

  /**
   * Used to execute an action.
   *
   * @param evt The event to execute.
   * @param ctx The context for the state machine.
   * @param param The parameter that was passed to the action.
   */
  execute(evt: TEvent, ctx: TCtx, param?: TParam);
}
