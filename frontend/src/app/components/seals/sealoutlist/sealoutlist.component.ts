import { Component, OnInit, ViewEncapsulation, ViewChild, Input, ElementRef } from "@angular/core";

import {
  NgbDateStruct,
  NgbModal,
  NgbModalRef,
  NgbModalOptions,
  ModalDismissReasons,
} from "@ng-bootstrap/ng-bootstrap";
import { RestService } from "../../../services/rest.service";
import { NgxSpinnerService } from "ngx-spinner";
import * as swalFunctions from "../../../shared/services/sweetalert.service";
import { th } from "date-fns/locale";
import { SealOut } from "../../../models/seal-out.model";
import { forEach } from "core-js/core/array";
import { RecriptComponent } from "./recript/recript.component";
import { map } from "rxjs/operators";
import { Seals } from "app/models/seals.model";
import { SealOutInfo } from "app/models/seal-out-info";
import { ToastrService } from "ngx-toastr";


@Component({
  selector: "app-sealoutlist",
  templateUrl: "./sealoutlist.component.html",
  styleUrls: [
    "./sealoutlist.component.scss",
    "../../../../assets/sass/libs/datepicker.scss",
  ],
  providers: [RestService],
})
export class SealOutListComponent implements OnInit {
  window: any;
  swal = swalFunctions;
  modalRef: NgbModalRef;
  private mediaQueryList: MediaQueryList;
  constructor(
    private modalService: NgbModal,
    private service: RestService,
    private spinner: NgxSpinnerService,
    public toastr: ToastrService,
  ) {}

  ngOnInit(): void {
    this.selectToday();
    this.getSeal();
    this.window = window;
    this.now = new Date();

  }

  displayMonths = 2;
  dtStart: NgbDateStruct;
  dtEnd: NgbDateStruct;
  page = 1;
  pageSize = 10;
  pageSizes = [10, 20, 50, 100];
  currentPage = 1;
  searchTerm: string = "";
  closeResult: string;
  checkedAll: boolean = false;
  sealNo: string;
  sealOutItem: SealOut[] = [];
  filterItems: SealOut[] = [];
  now:Date = new Date();
  columnSearch = "";
  isCancel:string = "";
  mSealOulItem:any[]=[];
  mSealList: Seals[] = [];
  itemSealChange:any[] = [];
  sealItem:any[] = [];
  sealRemarks:any[];

  sealId:number = 0;
  sealOutId:string = '';
  pdfUrl: string;
  invoiceno:string = '';
  @ViewChild('content') popupview !: ElementRef;
  @Input() txtSealId:number=3;
  user :string =this.service.getFullNameLocalAuthen();
  pageChanged(event: any): void {
    this.page = event.page;
  }
  clearTextSearch() {
    this.searchTerm = "";
    this.getSeal();
  }
  checkAllItems() {
    for (let item of this.sealOutItem) {
      if (this.checkedAll) {
        item.checked = true;
      } else {
        item.checked = false;
      }
    }
  }
  isDisabled(date: NgbDateStruct, current: { month: number }) {
    return date.month !== current.month;
  }

  // Selects today's date
  selectToday() {
    this.dtStart = {
      year: this.now.getFullYear(),
      month: this.now.getMonth() + 1,
      day: this.now.getDate(),
    };
    let tomorrow:Date = this.now;
    tomorrow.setDate(tomorrow.getDate() + 1)
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

  format(date: NgbDateStruct): string {
    return date
      ? date.year +
          "-" +
          ("0" + date.month).slice(-2) +
          "-" +
          ("0" + date.day).slice(-2)
      : null;
  }

  getSeal() {
    let startDate: string = `${this.dtStart.year}-${this.dtStart.month}-${this.dtStart.day}`;
    let endDate: string = `${this.dtEnd.year}-${this.dtEnd.month}-${this.dtEnd.day}`;
    this.service
      .getSealOutAll(this.isCancel,this.columnSearch, this.searchTerm, startDate, endDate).subscribe((res: any) => {
      this.sealOutItem = res.result;
    });
  }

  deleteData(id: any) {;
    this.swal
      .ConfirmText("แจ้งเตือนการลบข้อมูล", "คุณต้องการลบข้อมูลหรือไม่?")
      .then((res) => {
        if (res) {
          this.service.deleteSealOut(id).subscribe(
            (res: any) => {
              this.swal.showDialog("success", "ลบข้อมูลเรียบร้อยแล้วแล้ว");
              this.getSeal();
            },
            (error: any) => {
              this.swal.showDialog("error", "เกิดข้อผิดพลาด:" + error);
            }
          );
        }
      });
  }
  getSealOutItem(id:string){
    console.log(id);
    this.service.getSealOutItem(id).subscribe(
      data => {
        this.mSealOulItem = data.result;
      },
      error => {
        console.log(JSON.stringify(error));
      }
    );
  }
  showSealOutInfo(content:any,item: any) {
    //debugger;
    this.itemSealChange=[];
    this.getSealOutItem(item.sealOutId);
    this.sealOutId =item.sealOutId;
    const modalOptions: NgbModalOptions = {
      keyboard: false,
      centered: false,
      size: 'xl',
    };
    this.openModal(content,modalOptions);
  }
  openModal(content: any,modalOptions:any) {
    this.modalRef= this.modalService.open(content, modalOptions);
  }
  closeModal() {
    if (this.modalRef) {
      this.modalRef.close();
    }
  }
  printSlip(item: any) {
    let ngbModalOptions: NgbModalOptions = {
      backdrop: "static",
      size: "md",
    };
    const modalRef = this.modalService.open(RecriptComponent, ngbModalOptions);
    modalRef.componentInstance.sealOutId = item.sealOutId;
    //modalRef.componentInstance.data = item;
  }
  sleep(ms) {
    return new Promise((resolve) => setTimeout(resolve, ms));
  }
  getSealChange(){
    this.service.getSealChange().subscribe((data) =>{
      this.sealItem = data.result;
    });
  }
  getSealRemark(){
    this.service.getSealRemarks().subscribe((data) =>{
      this.sealRemarks = data.result;
    });
  }
  addItemSealChange(sealOutId:string,sealInId:string,item: any) {
    const result = this.itemSealChange.find((p) => p.sealNoOld=== item.sealNo);
    if (result){
      this.toastr.warning("มีหมายเลขซีลนี้ในตารางแล้ว");
      return false;
    }
    this.getSealChange();
    this.getSealRemark();
    this.itemSealChange.push({
      sealId: item.id,
      sealOutId: sealOutId,
      sealInId: sealInId,
      sealIdOld:item.sealId,
      sealNoOld:item.sealNo,
      sealIdNew:0,
      sealNoNew:'',
      remarkId:1,
      remarks:'',
      remarkOther:'',
      createdBy:this.user,
      updatedBy:this.user
    })
  }
  removeItem(item: any) {
    let index = this.itemSealChange.indexOf(item);
    this.itemSealChange.splice(index, 1);
  }
  selectRemark(id:string){
    console.log(id);
  }
  selectSealNew(item: any) {
    console.log(item);
  }
  previewReceipt(SealOutId: string) {
    this.service.GenerateInvoicePDF('IVN3576788').subscribe(res => {
      const blob = new Blob([res.body], { type: 'application/pdf' });
      this.pdfUrl = URL.createObjectURL(blob);
      this.modalService.open(this.popupview, { size: 'lg' });
      //window.open(url);
    });
  }
  submitFormSealChange(){
    if (this.itemSealChange.length===0){
      this.swal.showDialog("warning", "การแจ้งเตือน:กรุณาเลือกหมายเลขซีลที่ต้องการเปลี่ยนด้วยครับ");
      return;
    }
    const result = this.itemSealChange.find((p) => p.sealIdNew ==0);
    if (result) {
      this.toastr.warning("กรุณากรอก หมายเลขซีลใหม่ด้วยครับ");
      return;
    }

    this.service.sealChange(this.itemSealChange).subscribe(
      (res: any) => {
        this.spinner.hide();
        this.closeModal();
        if (res.success) {
          this.swal.showDialog("success", res.message);
        } else {
          this.swal.showDialog(
            "warning",
            "เกิดข้อผิดพลาด : " + res.message
          );
        }
      },
      (error: any) => {
        this.closeModal();
        this.spinner.hide();
        this.swal.showDialog("error", "เกิดข้อผิดพลาด : " + error);
      }
    );
  }
}
