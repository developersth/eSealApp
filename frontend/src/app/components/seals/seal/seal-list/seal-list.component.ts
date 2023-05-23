import { Component, OnInit } from '@angular/core';
import * as swalFunctions from "../../../../shared/services/sweetalert.service";
import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { RestService } from 'app/services/rest.service';
@Component({
  selector: 'app-seal-list',
  templateUrl: './seal-list.component.html',
  styleUrls: ['./seal-list.component.scss','../../../../../assets/sass/libs/datepicker.scss'],
  providers: [RestService],
})
export class SealListComponent implements OnInit {
  data: any[];
  swal = swalFunctions;
  pageSize:number =10;
  pageSizes = [10, 20, 50, 100];
  currentPage = 1;
  displayMonths = 2;
  dtStart: NgbDateStruct;
  dtEnd: NgbDateStruct;
  filterItems:any[] = [];
  keyword:string = "";
  now: Date = new Date();
  constructor(private service:RestService) { }

  ngOnInit(): void {
    this.selectToday();
    this.getData();
  }
  selectToday() {
    this.dtStart = {
      year: this.now.getFullYear(),
      month: this.now.getMonth() + 1,
      day: this.now.getDate(),
    };
    let tomorrow: Date = this.now;
    tomorrow.setDate(tomorrow.getDate() + 1);
    this.dtEnd = {
      year: tomorrow.getFullYear(),
      month: tomorrow.getMonth() + 1,
      day: tomorrow.getDate(),
    };
  }
    // Custom Day View Starts
    isWeekend(date: NgbDateStruct) {
      const d = new Date(date.year, date.month - 1, date.day);
      return d.getDay() === 0 || d.getDay() === 6;
    }
  isDisabled(date: NgbDateStruct, current: { month: number }) {
    return date.month !== current.month;
  }
  addData(){

  }
  searchItem() {
    this.currentPage = 1; // รีเซ็ตหน้าเป็นหน้าที่ 1 เมื่อทำการกรองข้อมูล
    this.keyword = this.keyword.trim().toLowerCase();
    if (this.keyword === "") {
      // กรณีไม่มีคำค้นหา ให้แสดงข้อมูลทั้งหมด
      this.filterItems = this.data;
    } else {
      // กรณีมีคำค้นหา ให้กรองข้อมูลตามคำค้นหา
      this.filterItems = this.data.filter(
        (item) =>
          item.id.toString().includes(this.keyword) ||
          item.sealNo.toString().includes(this.keyword) ||
          item.typeName.toString().includes(this.keyword) ||
          item.statusName.toString().includes(this.keyword) ||
          item.createdBy.toString().includes(this.keyword) ||
          item.updatedBy.toString().includes(this.keyword)
      );
    }
  }
  getData(){
    let startDate: string = `${this.dtStart.year}-${this.dtStart.month}-${this.dtStart.day}`;
    let endDate: string = `${this.dtEnd.year}-${this.dtEnd.month}-${this.dtEnd.day}`;
    this.service.getSeals(startDate,endDate).subscribe((res: any) => {
      this.data = res.result;
      this.searchItem();
    });
  }
  clearTextSearch(){
    this.keyword ='';
  }

}
