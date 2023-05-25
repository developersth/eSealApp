import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { SealinComponent } from './sealin/sealin.component';
import { SealoutComponent } from './sealout/sealout.component';
import { SealOutListComponent } from './sealoutlist/sealoutlist.component';
import { SealListComponent } from './seal/seal-list/seal-list.component';
import { AuthGuard } from 'app/shared/auth/auth-guard.service';
import { CheckCancelFormGuard } from 'app/shared/auth/check-cancel-form.guard';
const routes: Routes = [
  {
    path: '',
    children: [
      {
        path: 'sealin',
        component: SealinComponent,
        data: {
          title: 'รายการซีลเข้าระบบ'
        }
      },
      {
        path: 'sealout',
        component: SealoutComponent,
        data: {
          title: 'การจ่ายซีล'
        }
      },
      {
        path: 'sealout/:id',
        component: SealoutComponent,
        data: {
          title: 'การจ่ายซีล'
        }
      },
      { path: 'sealout/:id', component: SealoutComponent, canActivate: [AuthGuard], canDeactivate: [CheckCancelFormGuard]},
      {
        path: 'sealoutlist',
        component: SealOutListComponent,
        data: {
          title: 'รายการจ่ายซีล'
        }
      },
      {
        path: 'seallist',
        component: SealListComponent,
        data: {
          title: 'ข้อมูลซีล'
        }
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class SealsRouteModule { }
