import { Component } from '@angular/core';
import {TestMessageService} from './share/services/test-message.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'webSocket';
  errors: string[];

  constructor(
    private testService: TestMessageService
  ) {
  }

  sendTestMessage() {
    this.testService.sendTest().subscribe(result => {
      result.error((errors) => {
        this.errors = errors;
      });
    });
  }
}
