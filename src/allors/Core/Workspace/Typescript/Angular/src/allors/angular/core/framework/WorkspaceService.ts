import { Injectable } from '@angular/core';
import { domain } from '../../../domain';
import { MetaPopulation, Workspace } from '../../../framework';
import { data } from '../../../meta';

@Injectable()
export class WorkspaceService {
  public metaPopulation: MetaPopulation;
  public workspace: Workspace;

  constructor() {
    this.metaPopulation = new MetaPopulation(data);
    this.workspace = new Workspace(this.metaPopulation);
    domain.apply(this.workspace);
  }
}
