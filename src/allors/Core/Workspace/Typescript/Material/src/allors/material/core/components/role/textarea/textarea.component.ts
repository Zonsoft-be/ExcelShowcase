import { Component, Optional, ViewChild, NgZone } from '@angular/core';
import { NgForm } from '@angular/forms';
import { CdkTextareaAutosize } from '@angular/cdk/text-field';

import { Test, RoleField } from '../../../../../angular';
import { take } from 'rxjs/operators';

@Component({
  // tslint:disable-next-line:component-selector
  selector: 'a-mat-textarea',
  templateUrl: './textarea.component.html',
})
@Test
export class AllorsMaterialTextareaComponent extends RoleField {

  @ViewChild('autosize') autosize: CdkTextareaAutosize;

  constructor(@Optional() parentForm: NgForm, private ngZone: NgZone) {
    super(parentForm);
  }

  triggerResize() {
    // Wait for changes to be applied, then trigger textarea resize.
    this.ngZone.onStable.pipe(take(1))
      .subscribe(() => this.autosize.resizeToFitContent(true));
  }
}
