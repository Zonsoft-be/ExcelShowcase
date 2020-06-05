import { Component, EventEmitter, Input, Optional, Output } from '@angular/core';
import { NgForm } from '@angular/forms';

import { ISessionObject } from '../../../../../framework';

import { RoleField } from '../../../../../angular';

@Component({
  // tslint:disable-next-line:component-selector
  selector: 'a-mat-select',
  templateUrl: './select.component.html',
})
export class AllorsMaterialSelectComponent extends RoleField {
  @Input()
  public display = 'display';

  @Input()
  public options: ISessionObject[];

  @Output()
  public selected: EventEmitter<ISessionObject> = new EventEmitter();

  constructor(@Optional() parentForm: NgForm) {
    super(parentForm);
  }

  public onModelChange(option: ISessionObject): void {
    this.selected.emit(option);
  }
}
