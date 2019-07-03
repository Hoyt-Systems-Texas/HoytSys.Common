import {SkipQueueRingBuffer} from './SkipQueueRingBuffer';

describe('Skip Buffer test', () => {
  beforeEach(() => {

  });

  describe('Offer test', () => {
    it('Test basic add take', () => {
      const buffer = new SkipQueueRingBuffer<number>(2);
      expect(buffer.offer(1)).toBeTruthy();
      expect(buffer.offer(2)).toBeTruthy();
      expect(buffer.offer(3)).toBeFalsy();
    });
  });

  describe('Poll/Offer test', () => {
    it('Test poll', () => {
      const buffer = new SkipQueueRingBuffer<number>(2);
      expect(buffer.offer(1)).toBeTruthy();
      expect(buffer.offer(2)).toBeTruthy();
      const r = buffer.poll();
      expect(r[0]).toBeTruthy();
      expect(r[1]).toBe(1);
      expect(buffer.offer(3)).toBeTruthy();
    });
  });

  describe('Skip/Offer test', () => {
    it('Test Skip', () => {
      const buffer = new SkipQueueRingBuffer(2);
      expect(buffer.offer(1)).toBeTruthy();
      expect(buffer.offer(2)).toBeTruthy();
      buffer.skip();
      expect(buffer.offer(3)).toBeFalsy();
      let take = buffer.poll();
      expect(take[0]).toBeTruthy();
      expect(take[1]).toBe(2);
      expect(buffer.offer(3)).toBeFalsy();
      buffer.resetToFirst();
      take = buffer.poll();
      expect(take[0]).toBeTruthy();
      expect(take[1]).toBe(1);
      expect(buffer.offer(3)).toBeTruthy();
      expect(buffer.offer(4)).toBeTruthy();
      expect(buffer.offer(5)).toBeFalsy();
      take = buffer.poll();
      expect(take[0]).toBeTruthy();
      expect(take[1]).toBe(3);
    });
  });
});
