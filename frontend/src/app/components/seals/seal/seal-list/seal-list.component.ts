import { Component, OnInit } from '@angular/core';
import * as swalFunctions from "../../../../shared/services/sweetalert.service";
import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { RestService } from 'app/services/rest.service';
import { NgbModal, NgbModalOptions } from "@ng-bootstrap/ng-bootstrap";
import { NgxSpinnerService } from "ngx-spinner";
import { SealModalComponent } from '../seal-modal/seal-modal.component';
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
  constructor(
    private service:RestService,
    private modalService: NgbModal,
    private spinner: NgxSpinnerService
    ) { }

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
  getColor(status:number):string{
    if(status ===1){
      return 'success';
    }else if(status ===2){
      return 'danger';
    }else{
      return 'secondary';
    }
  }
  addData() {
    let ngbModalOptions: NgbModalOptions = {
      backdrop: "static",
      size: "md",
    };
    const modalRef = this.modalService.open(SealModalComponent, ngbModalOptions);
    modalRef.componentInstance.id = 0; // should be the id
    modalRef.componentInstance.data = {
      sealNo: '',
      type: 0,
      status: 0,
      isActive: 0,
    }; // should be the data
    modalRef.result
      .then((result) => {
        this.spinner.show(undefined, {
          type: "ball-triangle-path",
          size: "medium",
          bdColor: "rgba(0, 0, 0, 0.8)",
          color: "#fff",
          fullScreen: true,
        });
        //initial user for body
        const body = {
          sealNo: result.sealNo,
          type: result.type,
          status: result.status,
          isActive:result.isActive,
          createdBy: this.service.getFullNameLocalAuthen(),
          updatedBy: this.service.getFullNameLocalAuthen(),
        };
        this.service.addSeal(body).subscribe(
          (res: any) => {
            this.spinner.hide();
            this.swal.showDialog("success", "เพิ่มข้อมูลสำเร็จแล้ว");
            this.getData();
          },
          (error: any) => {
            this.spinner.hide();
            this.swal.showDialog("error", "เกิดข้อผิดพลาด : " + error);
          }
        );
      })
      .catch((error) => {
        console.log(error);
      });
  }
  editData(item:any) {
    let ngbModalOptions: NgbModalOptions = {
      backdrop: "static",
      size: "md",
    };
    const modalRef = this.modalService.open(SealModalComponent, ngbModalOptions);
    modalRef.componentInstance.id = item.id; // should be the id
    modalRef.componentInstance.data = item;
    modalRef.result
      .then((result) => {
        this.spinner.show(undefined, {
          type: "ball-triangle-path",
          size: "medium",
          bdColor: "rgba(0, 0, 0, 0.8)",
          color: "#fff",
          fullScreen: true,
        });
        //initial user for body
        const body = {
          sealNo: result.sealNo,
          type: result.type,
          status: result.status,
          isActive:result.isActive,
          updatedBy: this.service.getFullNameLocalAuthen(),
        };
        this.service.updateSeal(item.id,body).subscribe(
          (res: any) => {
            this.spinner.hide();
            this.swal.showDialog("success", "แก้ไขข้อมูลสำเร็จแล้ว");
            this.getData();
          },
          (error: any) => {
            this.spinner.hide();
            this.swal.showDialog("error", "เกิดข้อผิดพลาด : " + error);
          }
        );
      })
      .catch((error) => {
        console.log(error);
      });
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
          item.id?.toString().toLowerCase().includes(this.keyword) ||
          item.sealNo?.toString().toLowerCase().includes(this.keyword) ||
          item.typeName?.toString().toLowerCase().includes(this.keyword) ||
          item.statusName?.toString().toLowerCase().includes(this.keyword) ||
          item.createdBy?.toString().toLowerCase().includes(this.keyword) ||
          item.updatedBy?.toString().toLowerCase().includes(this.keyword)
      );
    }
  }
  getData(){
    let startDate: string = `${this.dtStart.year}-${this.dtStart.month}-${this.dtStart.day}`;
    let endDate: string = `${this.dtEnd.year}-${this.dtEnd.month}-${this.dtEnd.day}`;
    this.spinner.show(undefined, {
      type: "ball-triangle-path",
      size: "medium",
      bdColor: "rgba(0, 0, 0, 0.8)",
      color: "#fff",
      fullScreen: true,
    });
    this.service.getSeals(startDate,endDate).subscribe((res: any) => {
      this.spinner.hide();
      this.data = res.result;
      this.searchItem();
    }, (err: any) => {
      this.spinner.hide();
    });
  }
  clearTextSearch(){
    this.keyword ='';
    this.getData();
  }

}
