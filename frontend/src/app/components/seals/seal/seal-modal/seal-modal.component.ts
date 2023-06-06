import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { RestService } from "app/services/rest.service";
import { ToastrService } from "ngx-toastr";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
@Component({
  selector: "app-seal-modal",
  templateUrl: "./seal-modal.component.html",
  styleUrls: ["./seal-modal.component.scss"],
  providers: [RestService],
})
export class SealModalComponent implements OnInit {
  id: number = 0;
  form: FormGroup;
  types: any[];
  status: any[];
  data:any[];
  constructor(
    public activeModal: NgbActiveModal,
    public formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private service: RestService,
    public toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.buildItemForm(this.data);
    this.types = [
      { id: 1, name: "ปกติ" },
      { id: 2, name: "พิเศษ" },
    ];
    this.status = [
      { id: 1, name: "พร้อมใช้งาน" },
      { id: 2, name: "ซีลชำรุด" },
      { id: 3, name: "ซีลสูญหาย" },
      { id: 4, name: "ซีลทดแทน" },
      { id:5, name: "ตีซีลผิดช่อง" }
    ];
  }
  private buildItemForm(item) {
    this.form = this.formBuilder.group({
      sealNo: [item.sealNo || "", Validators.required],
      type: [item.type||1],
      status: [item.status||1],
      isActive: [item.isActive||false]
    });
  }
  onSubmit() {
    if(this.form.value.sealNo===''){
      this.toastr.warning("กรุณาระบุหมายเลขซีล ด้วยครับ");
      return;
    }
    this.form.value.sealNo = this.form.value.sealNo.trim();
    this.activeModal.close(this.form.value);
  }
}
