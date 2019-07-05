import { TestBed } from '@angular/core/testing';

import { TestMessageService } from './test-message.service';

describe('TestMessageService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: TestMessageService = TestBed.get(TestMessageService);
    expect(service).toBeTruthy();
  });
});
