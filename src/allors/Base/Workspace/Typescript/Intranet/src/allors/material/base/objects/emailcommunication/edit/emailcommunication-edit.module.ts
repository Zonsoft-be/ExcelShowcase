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
import { MatTabsModule } from '@angular/material/tabs';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';

import { AllorsMaterialChipsModule } from '../../../../core/components/role/chips';
import { AllorsMaterialFooterModule } from '../../../../core/components/footer';
import { AllorsMaterialDatepickerModule } from '../../../../core/components/role/datepicker';
import { AllorsMaterialDatetimepickerModule } from '../../../../core/components/role/datetimepicker';
import { AllorsMaterialFileModule } from '../../../../core/components/role/file';
import { AllorsMaterialInputModule } from '../../../../core/components/role/input';
import { AllorsMaterialSelectModule } from '../../../../core/components/role/select';
import { AllorsMaterialSideNavToggleModule } from '../../../../core/components/sidenavtoggle';
import { AllorsMaterialSlideToggleModule } from '../../../../core/components/role/slidetoggle';
import { AllorsMaterialStaticModule } from '../../../../core/components/role/static';
import { AllorsMaterialTextAreaModule } from '../../../../core/components/role/textarea';

import { PersonInlineModule } from '../../person/inline/person-inline.module';
import { EmailAddressInlineModule } from '../../emailaddress/inline/emailaddress-inline.module';

import { EmailCommunicationEditComponent } from './emailcommunication-edit.component';
export { EmailCommunicationEditComponent } from './emailcommunication-edit.component';

@NgModule({
  declarations: [
    EmailCommunicationEditComponent,
  ],
  exports: [
    EmailCommunicationEditComponent,
  ],
  imports: [
    AllorsMaterialChipsModule,
    AllorsMaterialDatepickerModule,
    AllorsMaterialDatetimepickerModule,
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
    MatTabsModule,
    MatToolbarModule,
    MatTooltipModule,
    MatOptionModule,
    EmailAddressInlineModule,
    PersonInlineModule,
    ReactiveFormsModule,
    RouterModule,
  ],
})
export class EmailCommunicationEditModule { }
