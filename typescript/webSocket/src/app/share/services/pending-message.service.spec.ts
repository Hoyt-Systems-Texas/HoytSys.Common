import { TestBed } from '@angular/core/testing';

import { PendingMessageService } from './pending-message.service';

describe('PendingMessageService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: PendingMessageService = TestBed.get(PendingMessageService);
    expect(service).toBeTruthy();
  });
});
