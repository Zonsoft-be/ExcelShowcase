import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatOptionModule } from '@angular/material/core';
import { MatDialogModule } from '@angular/material/dialog';
import { MatDividerModule } from '@angular/material/divider';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';

import { AllorsMaterialDatepickerModule } from '../../../../core/components/role/datepicker';
import { AllorsMaterialFileModule } from '../../../../core/components/role/file';
import { AllorsMaterialFooterModule } from '../../../../core/components/footer';
import { AllorsMaterialInputModule } from '../../../../core/components/role/input';
import { AllorsMaterialSideNavToggleModule } from '../../../../core/components/sidenavtoggle';
import { AllorsMaterialSelectModule } from '../../../../core/components/role/select';
import { AllorsMaterialSlideToggleModule } from '../../../../core/components/role/slidetoggle';
import { AllorsMaterialStaticModule } from '../../../../core/components/role/static';
import { AllorsMaterialTextAreaModule } from '../../../../core/components/role/textarea';

import { CustomerRelationshipEditComponent } from './customerrelationship-edit.component';
export { CustomerRelationshipEditComponent } from './customerrelationship-edit.component';

@NgModule({
  declarations: [
    CustomerRelationshipEditComponent,
  ],
  exports: [
    CustomerRelationshipEditComponent,
  ],
  imports: [
    AllorsMaterialDatepickerModule,
    AllorsMaterialFileModule,
    AllorsMaterialFooterModule,
    AllorsMaterialInputModule,
    AllorsMaterialSelectModule,
    AllorsMaterialSideNavToggleModule,
    AllorsMaterialSlideToggleModule,
    AllorsMaterialStaticModule,
    AllorsMaterialTextAreaModule,
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatCardModule,
    MatDialogModule,
    MatDividerModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatListModule,
    MatMenuModule,
    MatRadioModule,
    MatSelectModule,
    MatToolbarModule,
    MatTooltipModule,
    MatOptionModule,
    ReactiveFormsModule,
    RouterModule,
  ],
})
export class CustomerRelationshipEditModule { }
