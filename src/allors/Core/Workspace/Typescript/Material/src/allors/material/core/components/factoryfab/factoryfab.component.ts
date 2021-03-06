import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

import { ObjectType, IObject } from '../../../../framework';
import { DatabaseService, WorkspaceService, Context } from '../../../../angular';

import { ObjectData, ObjectService } from '../../services/object';

@Component({
  // tslint:disable-next-line:component-selector
  selector: 'a-mat-factory-fab',
  templateUrl: './factoryfab.component.html',
  styleUrls: ['./factoryfab.component.scss']
})
export class FactoryFabComponent implements OnInit {

  @Input() private objectType: ObjectType;

  @Input() private createData: ObjectData;

  @Output() private created: EventEmitter<IObject> = new EventEmitter();

  classes: ObjectType[];

  constructor(
    public readonly factoryService: ObjectService,
    private databaseService: DatabaseService,
    private workspaceService: WorkspaceService,
) {
  }

  ngOnInit(): void {

    if (this.objectType.isInterface) {
      this.classes = this.objectType.classes;
    } else {
      this.classes = [this.objectType];
    }

    const context = new Context(this.databaseService.database, this.workspaceService.workspace);
    this.classes = this.classes.filter((v) => this.factoryService.hasCreateControl(v, this.createData, context));
  }

  get dataAllorsActions(): string {
    return (this.classes) ? this.classes.map(v => v.name).join() : '';
  }

  create(objectType: ObjectType) {
    this.factoryService.create(objectType, this.createData)
      .subscribe((v) => {
        if (v && this.created) {
          this.created.next(v);
        }
      });
  }
}
