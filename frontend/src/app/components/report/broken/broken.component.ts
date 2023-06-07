import { Component, OnInit } from "@angular/core";
import { RestService } from "app/services/rest.service";
import { NgbDateStruct, NgbModal, NgbModalOptions } from "@ng-bootstrap/ng-bootstrap";
import { NgxSpinnerService } from "ngx-spinner";
import * as swalFunctions from "../../../shared/services/sweetalert.service";

@Component({
  selector: 'app-broken',
  templateUrl: './broken.component.html',
  styleUrls: ['./broken.component.css',
  "../../../../assets/sass/libs/datepicker.scss",
],
  providers:[RestService]
})
export class BrokenComponent implements OnInit {
  sealchanges: any[];
  dtStart: NgbDateStruct;
  dtEnd: NgbDateStruct;
  now: Date = new Date();

  constructor(
    private service: RestService,
    private modalService: NgbModal,
    private spinner: NgxSpinnerService
  ) { }

  ngOnInit(): void {
    this.selectToday(); 
    this.getSealChanges();
  }

  getSealChanges() {
    let startDate: string = `${this.dtStart.year}-${this.dtStart.month}-${this.dtStart.day}`;
    let endDate: string = `${this.dtEnd.year}-${this.dtEnd.month}-${this.dtEnd.day}`;
    this.service.getSealChanges(startDate, endDate).subscribe((sealchange:any) => {
      this.sealchanges = sealchange.result;
      console.log(this.sealchanges);
    });
  }

  isDisabled(date: NgbDateStruct, current: { month: number }) {
    return date.month !== current.month;
  }
  
  isWeekend(date: NgbDateStruct) {
    const d = new Date(date.year, date.month - 1, date.day);
    return d.getDay() === 0 || d.getDay() === 6;
  }
  // Selects today's date
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

}
