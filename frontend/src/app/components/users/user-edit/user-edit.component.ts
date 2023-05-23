import { Component, Input, NgZone, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { User } from "../../../models/user.model";
import { RestService } from "../../../services/rest.service";
import {
  FormBuilder,
  FormGroup,
  UntypedFormControl,
  UntypedFormGroup,
  Validators,
} from "@angular/forms";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { th } from "date-fns/locale";
import { ToastrService } from "ngx-toastr";
@Component({
  selector: "app-user-edit",
  templateUrl: "./user-edit.component.html",
  styleUrls: ["./user-edit.component.scss"],
  providers:[RestService]
})
export class UserEditComponent implements OnInit {
  userForm: FormGroup;
  id: string = "";
  user: User[];
  @Input() data: any = {};
  rols:any[]
  constructor(
    public activeModal: NgbActiveModal,
    public formBuilder: FormBuilder,
    private router: Router,
    private ngZone: NgZone,
    private route: ActivatedRoute,
    private service: RestService,
    private toastrService: ToastrService
  ) {
    this.userForm = this.formBuilder.group({
      username: ["", Validators.required],
      password: [""],
      name: [""],
      email: [""],
      roleId: [0],
      isActive: [true],
    });
  }
  ngOnInit(): void {
    console.log(this.data);
    this.buildItemForm(this.data);
    this.rols=[
      {
        id:1,
        name:"ผู้ดูแลระบบ"
      },
      {
        id:2,
        name:"ผู้ใช้งาน"
      },
      {
        id:3,
        name:"หัวหน้างาน"
      }
    ]

  }
  private buildItemForm(item) {
    this.userForm = this.formBuilder.group({
      username: [item.username || "", Validators.required],
      password: [""],
      name: [item.name || ""],
      email: [item.email || ""],
      roleId: [item.roleId||""],
      isActive: [item.isActive],
    });
  }
  getUserAll() {
    const id = +this.route.snapshot.paramMap.get("id");
    this.service.getUser(id).subscribe((user: any) => {
      this.userForm = user;
    });
  }
  onSubmit() {
    if(this.userForm.get('username').value===''){
      this.toastrService.warning("กรุณาระบุ ชื่อผู้ใช้งาน");
      return;
    }
    if(this.userForm.get('name').value===''){
      this.toastrService.warning("กรุณาระบุ ชื่อ-นามสกุล");
      return;
    }
    this.activeModal.close(this.userForm.value);
  }
}
