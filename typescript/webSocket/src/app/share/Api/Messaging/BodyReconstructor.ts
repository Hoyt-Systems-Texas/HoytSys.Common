export class BodyReconstructor {

  received = 0;
  fragments: string[] = [];

  constructor(
    public readonly total: number
  ) {

  }

  append(position: number, body: string) {
    if (!this.fragments[position]) {
      this.fragments[position] = body;
      this.received++;
    }
  }

  missingFragments(handler: (num: number) => void) {
    for (let i = 0 ; i < this.total ; i++) {
      if (!this.fragments[i]) {
        handler(i);
      }
    }
  }

  isCompleted() {
    return this.received === this.total;
  }

  get body(): string {
    if (this.isCompleted()) {
      return this.fragments.reduce((p, c) => {
        return p + c;
      });
    }
    return null;
  }
}
