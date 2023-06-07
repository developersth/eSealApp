import { Component, Output, EventEmitter, Input, OnInit } from "@angular/core";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { NgxSpinnerService } from "ngx-spinner";
import { delay } from 'rxjs/operators';
import { ToastrService } from "ngx-toastr";
import { Observable, of } from "rxjs";
import { th } from "date-fns/locale";
import { Utils } from "../../../../utils/utils";
@Component({
  selector: "app-crud-modal",
  templateUrl: "./crud-modal.component.html",
  styleUrls: ["./crud-modal.component.scss"],
  providers:[Utils]
})
export class CrudModalComponent implements OnInit {
  type = "Marketing";
  @Input() id: number;
  @Input() sealTotal: string;
  @Input() sealStartNo: string;
  @Input() data: any[] = [];
  //@Input() data:Seal[]=[];
  private sealPack: number[] = [3, 4, 5];
  private seals: any[] = [];
  @Input() cbSealPack: string;
  Object: any;
  sealItem: any[] = [];
  constructor(
    public activeModal: NgbActiveModal,
    private spinner: NgxSpinnerService,
    private toast: ToastrService,
    private util:Utils
  ) { }

  ngOnInit() {
    this.data = []
    this.cbSealPack = "1";
  }

  CheckInvalid(){
    if(this.sealStartNo == null || this.sealTotal == null) {
      if (this.sealStartNo == null && this.sealTotal == null){
        this.toast.warning("กรุณากรอกข้อมูล");
      }
      else if(this.sealTotal == null){
        this.toast.warning("กรุณากรอกจำนวนซีล");
      }
      else{
        this.toast.warning("กรุณาหมายเลขซีล");
      }
      return false;
    }
    if (parseInt(this.sealTotal) > 1000) {
      this.toast.warning("ระบุจำนวนซีลไม่เกิน 1000")
      return false;
    }
    if(parseInt(this.sealTotal)%parseInt(this.cbSealPack)!=0){
      this.toast.warning("กรุณากรอกจำนวนซีลให้สอดคล้องกับแพ็คซีล");
      return false;
    }
    return true;
  }

  calculateSeal() {
    if(this.CheckInvalid()){
      if (!this.data) return;
      this.spinner.show(undefined, {
        type: "ball-triangle-path",
        size: "medium",
        bdColor: "rgba(0, 0, 0, 0.8)",
        color: "#fff",
        fullScreen: true,
      });
      this.data =[];
      this.util.calculateSealNumber(parseInt(this.cbSealPack),parseInt(this.sealTotal), parseFloat(this.sealStartNo)).pipe(delay(200)).subscribe(
        (res: any) => {
          this.data = res;
          this.spinner.hide();
        },
        (error: any) => {
          console.log(error.message)
          this.spinner.hide();
        }
      );
    }
    else{
      return;
    }
  }

  removeData(item) {
    var index = this.data.indexOf(item);
    this.data.splice(index, 1);
  }

  submitForm() {
    // let body={
    //   sealBetween :this.data["sealBetween"],
    //   pack :this.data["pack"],
    //   isActive :this.data["isActive"],
    //   sealItem :this.data["sealItem"],
    // }
    if(this.CheckInvalid()){
      this.activeModal.close(this.data);
    }
    else{
      return;
    }
  }
}
