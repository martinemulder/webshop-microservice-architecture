import { BestelStatus } from './bestelStatus';

export class UpdateBestelStatus {
  status: BestelStatus;

  constructor(status: BestelStatus) {
    this.status = status;
  }
}
