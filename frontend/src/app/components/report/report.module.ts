import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule,FormControl } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { PipeModule } from 'app/shared/pipes/pipe.module';
import { NgxSpinnerModule } from 'ngx-spinner';
import { NgxPaginationModule } from 'ngx-pagination';
import { DatePipe } from '@angular/common';
import { NgSelectModule } from '@ng-select/ng-select';
import { ReportRouteModule } from './report-routing.module';
import { BrokenComponent } from './broken/broken.component';
import { RemainingComponent } from './remaining/remaining.component';
@NgModule({
  declarations: [
    BrokenComponent,
    RemainingComponent,
  ],
  imports: [
    CommonModule,
    FormsModule,
    NgxDatatableModule,
    PipeModule,
    ReactiveFormsModule,
    NgbModule,
    NgxSpinnerModule,
    DatePipe,
    NgxPaginationModule,
    ReportRouteModule

  ]
})
export class ReportModule { }
