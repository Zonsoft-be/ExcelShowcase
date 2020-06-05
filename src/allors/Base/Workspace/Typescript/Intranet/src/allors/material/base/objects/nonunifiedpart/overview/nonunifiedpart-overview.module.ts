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

import { ProductIdentificationPanelModule } from '../../productidentification/overview/panel/productIdentification-panel.module';
import { NonSerialisedInventoryItemOverviewPanelModule } from '../../nonserialisedinventoryitem/overview/panel/nonserialisedinventoryitem-overview-panel.module';
import { PriceComponentOverviewPanelModule } from '../../pricecomponent/overview/panel/pricecomponent-overview-panel.module';
import { SerialisedInventoryItemOverviewPanelModule } from '../../serialisedinventoryitem/overview/panel/serialisedinventoryitem-overview-panel.module';
import { SerialisedItemOverviewPanelModule } from '../../serialiseditem/overview/panel/serialiseditem-overview-panel.module';
import { SupplierOfferingOverviewPanelModule } from '../../supplieroffering/overview/panel/supplieroffering-overview-panel.module';

import { NonUnifiedPartOverviewSummaryModule } from './summary/nonunifiedpart-overview-summary.module';
import { NonUnifiedPartOverviewDetailModule } from './detail/nonunifiedpart-overview-detail.module';

import { NonUnifiedPartOverviewComponent } from './nonunifiedpart-overview.component';
export { NonUnifiedPartOverviewComponent } from './nonunifiedpart-overview.component';

@NgModule({
  declarations: [
    NonUnifiedPartOverviewComponent,
  ],
  exports: [
    NonUnifiedPartOverviewComponent,
  ],
  imports: [
    NonUnifiedPartOverviewSummaryModule,
    NonUnifiedPartOverviewDetailModule,

    ProductIdentificationPanelModule,
    NonSerialisedInventoryItemOverviewPanelModule,
    PriceComponentOverviewPanelModule,
    SerialisedInventoryItemOverviewPanelModule,
    SerialisedItemOverviewPanelModule,
    SupplierOfferingOverviewPanelModule,

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
export class NonUnifiedPartOverviewModule { }
