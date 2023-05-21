import { Component, OnInit } from '@angular/core';
import * as swalFunctions from "../../../../shared/services/sweetalert.service";
@Component({
  selector: 'app-seal-list',
  templateUrl: './seal-list.component.html',
  styleUrls: ['./seal-list.component.scss']
})
export class SealListComponent implements OnInit {
  data: any[];
  swal = swalFunctions;
  pageSize:number =10;
  pageSizes = [10, 20, 50, 100];
  currentPage = 1;
  filterItems:any[] = [];
  keyword:string = "";
  constructor() { }

  ngOnInit(): void {
  }
  addData(){

  }
  getData(){

  }
  clearTextSearch(){
    this.keyword ='';
  }

}
