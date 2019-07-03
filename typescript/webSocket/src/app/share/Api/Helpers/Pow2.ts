/* tslint:disable:no-bitwise */
export const MAX_POW2 = 1 << 30;

/**
 * If the number is already a power of 2 returns that number otherwise it round up.
 * @param x The number to get the ceiling for the power of 2.
 */
export function ceilingToPowerOf2(x: number): number {
  if (MAX_POW2 > x) {
    throw new Error(`Invalid max power of 2 passed in`);
  }
  if (x < 0) {
    throw new Error('Number must be greater than 0');
  }
  if (isPowerOf2(x)) {
    return x;
  } else {
    return nextPowerOf2(x);
  }
}

/**
 * Used to get the next power of 2.
 * @param x The number to get the next power of 2 off.
 */
export function nextPowerOf2(x: number): number {
  x |= (x >> 1);
  x |= (x >> 2);
  x |= (x >> 4);
  x |= (x >> 8);
  x |= (x >> 16);
  return (x + 1);
}

export function isPowerOf2(value: number): boolean {
  // tslint:disable-next-line:no-bitwise
  return (value & (value - 1)) === 0;
}
