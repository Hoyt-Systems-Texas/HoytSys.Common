/**
 * Base interface for the result monad.
 */
export interface IResultMonad<T> {
  bind<TB>(func: (value: T) => IResultMonad<TB>): IResultMonad<TB>;

  error(func: (errors: string[]) => void): IResultMonad<T>;

  busy(func: () => void): IResultMonad<T>;

  accessDenied(func: (errors: string[]) => void): IResultMonad<T>;

  allErrors(func: () => void): IResultMonad<T>;
}

/**
 * Used to create a result monad.
 * @param value The value to create.
 */
export function ToResult<T>(value: T): IResultMonad<T> {
  return new ResultSuccess(value);
}

export function ToError<T>(errors: string[]): IResultMonad<T> {
  return new ResultError(errors);
}

export class ResultSuccess<T> implements IResultMonad<T> {
  constructor(
    public readonly value: T
  ) {

  }

  accessDenied(func: (errors: string[]) => void) {
    return this;
  }

  bind<TB>(func: (value: T) => IResultMonad<TB>) {
    return func(this.value);
  }

  busy(func: () => void) {
    return this;
  }

  error(func: (errors: string[]) => void) {
    return this;
  }

  allErrors(func: () => void) {
    return this;
  }
}

export interface IResultError<T> extends IResultMonad<T> {

  to<TB>(): IResultMonad<TB>;
}

export abstract class BaseResultError<T> implements IResultError<T> {

  accessDenied(func: (errors: string[]) => void): IResultMonad<T> {
    return this;
  }

  allErrors(func: () => void): IResultMonad<T> {
    func();
    return this;
  }

  bind<TB>(func: (value: T) => IResultMonad<TB>): IResultMonad<TB> {
    return this.to();
  }

  busy(func: () => void): IResultMonad<T> {
    return this;
  }

  error(func: (errors: string[]) => void): IResultMonad<T> {
    return this;
  }

  to<TB>(): IResultMonad<TB> {
    return undefined;
  }

}

export class ResultError<T> extends BaseResultError<T> {

  constructor(
    public readonly errors: string[]
  ) {
    super();

  }

  error(func: (errors: string[]) => void): IResultMonad<T> {
    func(this.errors);
    return this;
  }

  to<TB>(): IResultMonad<TB> {
    return new ResultError(this.errors);
  }

}

export class ResultAccessDenied<T> extends BaseResultError<T> {

  constructor(
    public readonly errors: string[]
  ) {
    super();
  }

  accessDenied(func: (errors: string[]) => void): IResultMonad<T> {
    func(this.errors);
    return this;
  }

  to<TB>(): IResultMonad<TB> {
    return new ResultAccessDenied(this.errors);
  }
}

export class ResultBusy<T> extends BaseResultError<T> {

  busy(func: () => void): IResultMonad<T> {
    func();
    return this;
  }

  to<TB>(): IResultMonad<TB> {
    return new ResultBusy();
  }
}
