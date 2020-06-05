import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterModule } from '@angular/router';

import { AllorsMaterialFilterModule, AllorsMaterialTableModule, AllorsMaterialFactoryFabModule} from '../../../..';

import { PositionTypeRatesOverviewComponent } from './positiontyperate-list.component';
export { PositionTypeRatesOverviewComponent } from './positiontyperate-list.component';

@NgModule({
  declarations: [
    PositionTypeRatesOverviewComponent,
  ],
  exports: [
    PositionTypeRatesOverviewComponent,
  ],
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatIconModule,
    MatToolbarModule,
    MatTooltipModule,
    AllorsMaterialFactoryFabModule,
    AllorsMaterialFilterModule,
    AllorsMaterialTableModule,
  ],
})
export class PositionTypeRatesOverviewModule { }
