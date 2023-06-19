import {
  Component,
  Output,
  EventEmitter,
  Input,
  OnInit,
  ViewEncapsulation,
} from "@angular/core";
import { forEach } from "core-js/core/array";
import { RestService } from "../../../services/rest.service";
import { ToastrService } from "ngx-toastr";
import { th } from "date-fns/locale";
import { NgxSpinnerService } from "ngx-spinner";
import { ActivatedRoute, Router } from "@angular/router";
import {
  NgbDateStruct,
  NgbModal,
  NgbModalOptions,
  ModalDismissReasons,
} from "@ng-bootstrap/ng-bootstrap";
import { RecriptComponent } from "../sealoutlist/recript/recript.component";
import { TruckModalComponent } from "app/components/trucks/truck-modal/truck-modal.component";
import * as swalFunctions from "../../../shared/services/sweetalert.service";
import { NgOption } from "@ng-select/ng-select";
import { Truck } from "app/models/truck.model";
import { SealIn } from "app/models/seal-in.model";
import { stringify } from "querystring";
import { Seals } from "../../../models/seals.model";
import { Location } from '@angular/common';

@Component({
  selector: "app-sealout",
  templateUrl: "./sealout.component.html",
  styleUrls: [
    "./sealout.component.scss",
    "../../../../assets/sass/libs/select.scss",
  ],
  providers: [RestService],
  encapsulation: ViewEncapsulation.None,
})
export class SealoutComponent implements OnInit {
  getId: string;
  ngSelect: any;
  constructor(
    private router: Router,
    private service: RestService,
    public toastr: ToastrService,
    private spinner: NgxSpinnerService,
    private modalService: NgbModal,
    private activateRoute: ActivatedRoute,
    private location: Location
  ) {
    this.getId = this.activateRoute.snapshot.paramMap.get("id");
  }
  swal = swalFunctions;
  keyword = "name";
  @Input() txtSealTotal: number =0 ;
  @Input() txtSealTotalExtra: number =0 ;
  @Input() txtSealExtraTotal: number = 0;
  @Input() txtTruckId: string;
  @Input() sealItemExtraList: Seals[] = [];
  @Input() cbSealExtra:number = 0;
  selectedOptionsQRCode: any[] = [];
  itemSealBetWeen: any[] = [];
  itemSealOutList: any[] = [];
  itemSealExtra: any[] = [];
  mTruck: Truck[];
  mSealExtra:Seals[]=[];
  user:string=this.service.getFullNameLocalAuthen();
  //seal no item
  sealNoItem: any[] = [];

  clearSelectionQRCode() {
    this.selectedOptionsQRCode = [];
  }
  getSeaBetWeen() {
    this.service.getSeaBetWeen().subscribe((res: any) => {
      this.itemSealBetWeen = res.result;
    });
  }
  getTruck() {
    this.service.getTruck().subscribe(
      (res: any) => {
        this.mTruck = res.result;
      },
      (error) => {
        console.log(error);
      }
    );
  }
  isValidChkAddItemSeal(sealInId, pack) {
    const result = this.itemSealOutList.find((item) => item.sealInId === sealInId);
    if (result) {
      this.toastr.warning("มีหมายเลขซีลนี้ในตารางแล้ว");
      return false;
    }
    //check seal > total seal
    if (pack + this.subTotalSeal() > this.txtSealTotal) {
      this.toastr.warning("จำนวนซีลรวม มากกว่าซีลที่ต้องการ");
      return false;
    }
    return true;
  }
  getSealExtra(){
    this.service.getSealExtra().subscribe((res:any) => {
      this.itemSealExtra =res.result;
    });
  }
  selectTruckSeal() {
    if (this.txtTruckId) {
      const found = this.mTruck.find(
        (element) => element.truckId === this.txtTruckId
      );
      if (found) {
        this.txtSealTotal = found.sealTotal;
      }
    }
  }


  selectEvent() {
    let sealInId = this.selectedOptionsQRCode;
    this.clearSelectionQRCode();
    if (!this.txtSealTotal || this.txtSealTotal <= 0) {
      this.toastr.warning("กรุณาระบุ จำนวนซีล");
      return;
    }
    if (this.selectedOptionsQRCode) {
      const result = this.itemSealBetWeen.find((item) => item.sealInId === sealInId);
      console.log(result);
      if (!this.isValidChkAddItemSeal(sealInId, result.pack)) {
        return;
      }
      this.service.getSealItemBySealInId(result.sealInId).subscribe((res: any) => {
        this.itemSealOutList.push({
          sealInId: result.sealInId,
          sealBetween: result.sealBetween,
          pack: result.pack,
          sealType: 1,
          sealTypeName: "ปกติ",
          sealList:JSON.stringify(res.result)
        });

      });
    } else {
      this.toastr.warning("กรุณาเลือก หมายเลขซีล/QR Code");
    }
    // do something with selected item
  }
  selectSealExtra() {
    let id = this.cbSealExtra;
    const result = this.mSealExtra.find((item) => item.id === id);
    if (result) {
      this.toastr.warning("มีหมายเลขซีลนี้ในตารางแล้ว");
      return false;
    }
    this.service.getSealExtraById(id).subscribe((result:any) => {
      if (result) {
        this.mSealExtra.push(result.result[0]);
      }
    })
  }
  addListSealExtra() {
    this.getSealExtra();
    this.sealItemExtraList.push({
      id: 0,
      sealNo: "",
      status:1,
      type: "พิเศษ",
      createdBy:this.user,
      updatedBy:this.user
    });

  }
  removeItem(item: any) {
    let index = this.itemSealOutList.indexOf(item);
    this.itemSealOutList.splice(index, 1);
  }
  removeItemExtra(item: any) {
    let index = this.mSealExtra.indexOf(item);
    this.mSealExtra.splice(index, 1);
  }

  onChangeSearch(val: string) {
    console.log(val);
    // fetch remote data from here
    // And reassign the 'data' which is binded to 'data' property.
  }

  onFocused(e) {
    console.log("focused", e.value);
    // do something when input is focused
  }
  onKeyDownQrcode(txt: string) {
    console.log(txt);
  }
  subTotalSeal() {
    let total: number = 0;
    for (const key in this.itemSealOutList) {
      if (this.itemSealOutList[key].pack) {
        total += parseInt(this.itemSealOutList[key].pack);
      }
    }
    return total;
  }
  subTotalSealExtra() {
    let total: number = 0;
    for (const key in this.itemSealOutList) {
      if (this.itemSealOutList[key].sealType ===2) {
        total += parseInt(this.itemSealOutList[key].pack);
      }
    }
    return total;
  }
  private generator(): string {
    const isString = `${this.S4()}${this.S4()}-${this.S4()}-${this.S4()}-${this.S4()}-${this.S4()}${this.S4()}${this.S4()}`;

    return isString;
  }

  private S4(): string {
    return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
  }
  editSealNo(item: any) {
    if (item) {
      this.sealNoItem = [];
      item.forEach((element) => {
        this.sealNoItem.push({ sealNo: element.sealNo });
      });
    }
  }
  clearSealNoItem() {
    this.sealNoItem = [];
  }
  validateData() {
    //check จำนวนซีล
    if (!this.txtSealTotal || this.txtSealTotal === 0) {
      this.toastr.warning("กรุณากรอก จำนวนซีล");
      return false;
    }

    //check ทะเบียรถ
    if (!this.txtTruckId) {
      this.toastr.warning("กรุณาเลือก ทะเบียนรถ");
      return false;
    }
    //check item SealQrcode
    if (this.itemSealOutList.length === 0) {
      this.toastr.warning("กรุณาเลือก หมายเลขซีล/QR Code");
      return false;
    }
    //check count seal==txtSealTotal
    //seal extra
    const filterSealExtra = this.itemSealOutList.filter(
      (obj) => obj.sealType === 2
    );
    console.log(this.subTotalSealExtra());
    if (this.subTotalSeal() !== this.txtSealTotal + this.subTotalSealExtra()) {
      this.toastr.warning("จำนวนซีลไม่เท่ากับจำนวนซีลรวม");
      return false;
    }
    //check sealExtra ซ้ำ
    if (filterSealExtra.length > 0) {
      let valueArr = filterSealExtra.map(function (item) {
        return item.sealBetween;
      });
      let isDuplicate = valueArr.some(function (item, idx) {
        return valueArr.indexOf(item) != idx;
      });
      if (isDuplicate) {
        this.toastr.warning("หมายเลขซีลพิเศษซ้ำกัน");
        return false;
      }
    }
    //check ซีลพิเศษ
    if (filterSealExtra.length > 0) {
      const result = filterSealExtra.find((item) => item.sealBetween === "");
      if (result) {
        this.toastr.warning("กรุณากรอก หมายเลขซีลพิเศษ");
        return false;
      }
    }
    return true;
  }
  addTruck() {
    let ngbModalOptions: NgbModalOptions = {
      backdrop: "static",
      size: "md",
    };
    const modalRef = this.modalService.open(
      TruckModalComponent,
      ngbModalOptions
    );
    modalRef.componentInstance.id = ""; // should be the id
    modalRef.componentInstance.data = {
      truckIdHead: "",
      TruckIdTail: "",
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
        this.service.addTruck(result).subscribe(
          (res: any) => {
            this.spinner.hide();
            if (res.success) {
              this.swal.showDialog("success", res.message);
              this.getTruck();
            } else {
              this.swal.showDialog(
                "warning",
                "เกิดข้อผิดพลาด : " + res.message
              );
            }
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
  bindData() {
    if (this.getId) {
      this.service.getSealOutById(this.getId).subscribe((response: any) => {
        console.log(response);
        this.txtSealTotal = response.sealTotal;
        this.txtSealExtraTotal = response.sealTotalExtra;
        this.txtTruckId = response.truckId;
        this.itemSealOutList = response.sealItem;
      });
    }
  }
  onSubmit() {
    if (this.validateData() == false) return;
    if (this.getId) {
      this.editData();
    } else {
      this.addData();
    }
  }
  editData() {
    //validation before save
    if (!this.validateData()) return;

    this.spinner.show(undefined, {
      type: "ball-triangle-path",
      size: "medium",
      bdColor: "rgba(0, 0, 0, 0.8)",
      color: "#fff",
      fullScreen: true,
    });

    const result = this.mTruck.find((item) => item.truckId === this.txtTruckId);
    const body = {
      sealTotal: this.txtSealTotal,
      sealTotalExtra: this.txtSealExtraTotal,
      truckId: result.truckId,
      truckName: `${result.truckHead}/${result.truckTail}`,
      sealItemList:JSON.stringify(this.itemSealOutList)  ,
      sealExtraList:JSON.stringify(this.mSealExtra) ,
    };
    this.service.updateSealOut(this.getId, JSON.stringify(body)).subscribe(
      (res: any) => {
        this.spinner.hide();
        this.swal.showDialog("success", "แก้ไขข้อมูลสำเร็จแล้ว");
        this.showRecript(res);
        this.router.navigate(["/seals/sealoutlist"]);
      },
      (error: any) => {
        this.spinner.hide();
        this.swal.showDialog("error", "เกิดข้อผิดพลาด : " + error);
      }
    );
  }

  addData() {
    this.spinner.show(undefined, {
      type: "ball-triangle-path",
      size: "medium",
      bdColor: "rgba(0, 0, 0, 0.8)",
      color: "#fff",
      fullScreen: true,
    });

    const result = this.mTruck.find((item) => item.truckId === this.txtTruckId);
    let truckName = "";
    if (result.truckTail === "") truckName = result.truckHead;
    else truckName = `${result.truckHead} / ${result.truckTail}`;

    const body = {
      sealTotal: this.txtSealTotal,
      sealTotalExtra: this.mSealExtra.length,
      truckId: result.truckId,
      truckName: truckName,
      sealOutItem:this.itemSealOutList,
      sealExtraList:JSON.stringify(this.mSealExtra) ,
      createdBy: this.service.getFullNameLocalAuthen(),
      updatedBy: this.service.getFullNameLocalAuthen(),
    };
    console.log(body);
    this.service.addSealOut(JSON.stringify(body)).subscribe(
      (res: any) => {
        this.spinner.hide();
        this.swal.showDialog("success", "เพิ่มข้อมูลสำเร็จแล้ว");
        this.showRecript(res.result);
        this.router.navigate(["/seals/sealoutlist"]);
      },
      (error: any) => {
        this.spinner.hide();
        this.swal.showDialog("error", "เกิดข้อผิดพลาด : " + error);
      }
    );
  }
  showRecript(item: any) {
    let ngbModalOptions: NgbModalOptions = {
      backdrop: "static",
      size: "md",
    };
    const modalRef = this.modalService.open(RecriptComponent, ngbModalOptions);
    modalRef.componentInstance.id = item.id;
    modalRef.componentInstance.data = item;
  }
  ngOnInit(): void {
    this.getSeaBetWeen();
    this.getTruck();
    this.getSealExtra();
    this.bindData();
  }
  onClickBack() {
    this.location.back();
  }
}
