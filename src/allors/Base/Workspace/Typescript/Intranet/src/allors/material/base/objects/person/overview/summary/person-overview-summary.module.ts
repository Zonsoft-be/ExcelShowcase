import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatCardModule } from '@angular/material/card';
import { MatOptionModule } from '@angular/material/core';
import { MatDividerModule } from '@angular/material/divider';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';

import { AllorsMaterialFileModule } from '../../../../../core/components/role/file';
import { AllorsMaterialInputModule } from '../../../../../core/components/role/input';
import { AllorsMaterialSelectModule } from '../../../../../core/components/role/select';
import { AllorsMaterialSideNavToggleModule } from '../../../../../core/components/sidenavtoggle';
import { AllorsMaterialSlideToggleModule } from '../../../../../core/components/role/slidetoggle';
import { AllorsMaterialStaticModule } from '../../../../../core/components/role/static';
import { AllorsMaterialTextAreaModule } from '../../../../../core/components/role/textarea';

import { PersonOverviewSummaryComponent } from './person-overview-summary.component';
export { PersonOverviewSummaryComponent } from './person-overview-summary.component';

@NgModule({
  declarations: [
    PersonOverviewSummaryComponent,
  ],
  exports: [
    PersonOverviewSummaryComponent,
  ],
  imports: [
    AllorsMaterialFileModule,
    AllorsMaterialInputModule,
    AllorsMaterialSelectModule,
    AllorsMaterialSideNavToggleModule,
    AllorsMaterialSlideToggleModule,
    AllorsMaterialStaticModule,
    AllorsMaterialTextAreaModule,
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatButtonToggleModule,
    MatCardModule,
    MatDividerModule,
    MatExpansionModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatListModule,
    MatMenuModule,
    MatRadioModule,
    MatSelectModule,
    MatToolbarModule,
    MatTooltipModule,
    MatButtonToggleModule,
    MatOptionModule,
    ReactiveFormsModule,
    RouterModule,
  ],
})
export class PersonOverviewSummaryModule { }
