import { TestBed } from '@angular/core/testing';

import { SignalrConnectionService } from './signalr-connection.service';

describe('SignalrConnectionService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: SignalrConnectionService = TestBed.get(SignalrConnectionService);
    expect(service).toBeTruthy();
  });
});
