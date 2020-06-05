import { Injectable } from '@angular/core';
import { SessionState } from '../../core/state/SessionState';

@Injectable({
  providedIn: 'root',
})
export class InternalOrganisationId extends SessionState {
  constructor() {
    super('State$InternalOrganisationId');
  }
}
