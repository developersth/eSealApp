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
import { Roles } from "app/models/roles.model";
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
  roles: Roles[];

  @Input() data: any = {};
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
    this.buildItemForm(this.data);
    console.log(this.data);
    this.getRoles();
  }
  getRoles() {
    this.service.getRoles().subscribe((res:any) => {
      this.roles = res.result;
      //console.log(this.roles);
    });
  }

  private buildItemForm(item) {
    this.userForm = this.formBuilder.group({
      username: [item.username || "", Validators.required],
      password: [""],
      name: [item.name || ""],
      email: [item.email || ""],
      roleId: [item.roleId||0],
      isActive: [item.isActive],
    });


    //console.log(this.userForm.get('roleId').value);
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
