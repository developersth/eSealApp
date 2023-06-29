import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { RestService } from "app/services/rest.service";
import { ToastrService } from "ngx-toastr";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { SealTypes } from "app/models/seal-types.model";
import { SealStatus } from "app/models/seal-status.model";
@Component({
  selector: "app-seal-modal",
  templateUrl: "./seal-modal.component.html",
  styleUrls: ["./seal-modal.component.scss"],
  providers: [RestService],
})
export class SealModalComponent implements OnInit {
  id: number = 0;
  form: FormGroup;
  sealTypes: any[];
  sealStatus: SealStatus[];
  data: any[];
  constructor(
    public activeModal: NgbActiveModal,
    public formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private service: RestService,
    public toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.buildItemForm(this.data);
    this.getTypes();
    this.getStatus();
  }
  getTypes() {
    // this.service.getTypes().subscribe((res:any) => {
    //   this.sealTypes = res.result;
    // });
    this.sealTypes = [
      {
        typeName: "ปกติ",
      },
      {
        typeName: "พิเศษ",
      },
    ];
  }

  getStatus() {
    this.service.getStatus().subscribe((res: any) => {
      this.sealStatus = res.result;
    });
  }
  private buildItemForm(item) {
    this.form = this.formBuilder.group({
      sealNo: [item.sealNo || "", Validators.required],
      type: [item.type || 'ปกติ'],
      status: [item.status || 1],
      isActive: [item.isActive || false],
    });
  }
  onSubmit() {
    if (this.form.value.sealNo === "") {
      this.toastr.warning("กรุณาระบุหมายเลขซีล ด้วยครับ");
      return;
    }
    this.form.value.sealNo = this.form.value.sealNo.trim();
    this.activeModal.close(this.form.value);
  }
}
