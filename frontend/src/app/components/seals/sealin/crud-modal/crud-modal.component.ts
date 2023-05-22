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
  cbSealPack: number = 1;
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
  }



  calculateSeal() {
    if (parseInt(this.sealTotal) > 1000) {
      this.toast.warning("สามารถระบุจำนวนซีลได้ไม่เกิน 1000")
      return;
    }
    if (!this.data) return;
    this.spinner.show(undefined, {
      type: "ball-triangle-path",
      size: "medium",
      bdColor: "rgba(0, 0, 0, 0.8)",
      color: "#fff",
      fullScreen: true,
    });
    this.data =[];
    this.util.calculateSealNumber(this.cbSealPack,parseInt(this.sealTotal), parseFloat(this.sealStartNo)).pipe(delay(200)).subscribe(
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
    this.activeModal.close(this.data);
  }
}
