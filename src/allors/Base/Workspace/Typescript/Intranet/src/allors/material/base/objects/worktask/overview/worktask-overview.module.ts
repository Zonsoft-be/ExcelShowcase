import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatChipsModule } from '@angular/material/chips';
import { MatOptionModule } from '@angular/material/core';
import { MatDialogModule } from '@angular/material/dialog';
import { MatDividerModule } from '@angular/material/divider';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterModule } from '@angular/router';

import { AllorsMaterialFileModule } from '../../../../core/components/role/file';
import { AllorsMaterialHeaderModule } from '../../../../core/components/header';
import { AllorsMaterialInputModule } from '../../../../core/components/role/input';
import { AllorsMaterialLauncherModule } from '../../../..';
import { AllorsMaterialSelectModule } from '../../../../core/components/role/select';
import { AllorsMaterialSideNavToggleModule } from '../../../../core/components/sidenavtoggle';
import { AllorsMaterialSlideToggleModule } from '../../../../core/components/role/slidetoggle';
import { AllorsMaterialStaticModule } from '../../../../core/components/role/static';
import { AllorsMaterialTextAreaModule } from '../../../../core/components/role/textarea';

import { TimeEntryOverviewPanelModule } from '../../timeentry/overview/panel/timeentry-overview-panel.module';
import { WorkEffortOverviewPanelModule } from '../../workeffort/overview/panel/workeffort-overview-panel.module';
import { WorkEffortAssignmentRateOverviewPanelModule } from '../../workeffortassignmentrate/overview/panel/workeffortassignmentrate-overview-panel.module';
import { WorkEffortFixedAssetAssignmentOverviewPanelModule } from '../../workeffortfixedassetassignment/overview/panel/workeffortfixedassetassignment-overview-panel.module';
import { WorkEffortInventoryAssignmentOverviewPanelModule } from '../../workeffortinventoryassignment/overview/panel/workeffortinventoryassignment-overview-panel.module';
import { WorkEffortPartyAssignmentOverviewPanelModule } from '../../workeffortpartyassignment/overview/panel/workeffortpartyassignment-overview-panel.module';
import { WorkEffortPurchaseOrderItemAssignmentOverviewPanelModule } from '../../workeffortpoiassignment/overview/panel/workeffortpoiassignment-overview-panel.module';

import { WorkTaskOverviewSummaryModule } from './summary/worktask-overview-summary.module';
import { WorkTaskOverviewDetailModule } from './detail/worktask-overview-detail.module';

import { WorkTaskOverviewComponent } from './worktask-overview.component';
export { WorkTaskOverviewComponent } from './worktask-overview.component';

@NgModule({
  declarations: [
    WorkTaskOverviewComponent,
  ],
  exports: [
    WorkTaskOverviewComponent,
  ],
  imports: [
    WorkTaskOverviewSummaryModule,
    WorkTaskOverviewDetailModule,

    TimeEntryOverviewPanelModule,
    WorkEffortOverviewPanelModule,
    WorkEffortAssignmentRateOverviewPanelModule,
    WorkEffortFixedAssetAssignmentOverviewPanelModule,
    WorkEffortInventoryAssignmentOverviewPanelModule,
    WorkEffortPartyAssignmentOverviewPanelModule,
    WorkEffortPurchaseOrderItemAssignmentOverviewPanelModule,

    AllorsMaterialFileModule,
    AllorsMaterialHeaderModule,
    AllorsMaterialInputModule,
    AllorsMaterialLauncherModule,
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
    MatCheckboxModule,
    MatChipsModule,
    MatDialogModule,
    MatDividerModule,
    MatExpansionModule,
    MatFormFieldModule,
    MatGridListModule,
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
export class WorkTaskDetailModule { }
