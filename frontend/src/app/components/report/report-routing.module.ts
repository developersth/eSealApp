import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { BrokenComponent } from './broken/broken.component';
import { RemainingComponent } from './remaining/remaining.component';
const routes: Routes = [
  {
    path: '',
    children: [
      {
        path: 'remaining',
        component: RemainingComponent,
        data: {
          title: 'รายงานซีลชำรุด'
        }
      },
      {
        path: 'broken',
        component: BrokenComponent,
        data: {
          title: 'รายงานซีลชำรุด'
        }
      }
    ]
  }
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ReportRouteModule { }
