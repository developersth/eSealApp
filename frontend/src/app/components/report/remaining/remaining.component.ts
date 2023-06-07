import { Component, OnInit } from '@angular/core';
import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { RestService } from 'app/services/rest.service';

@Component({
  selector: 'app-remaining',
  templateUrl: './remaining.component.html',
  styleUrls: ['./remaining.component.css','../../../../assets/sass/libs/datepicker.scss']
})
export class RemainingComponent implements OnInit {

  dtStart: NgbDateStruct;
  dtEnd: NgbDateStruct;
  now: Date = new Date();
  data:any[];
  constructor(
    private service:RestService
  ) { }

  ngOnInit() {
    this.selectToday();
    this.getData();
  }
  getData()
  {
    let startDate: string = `${this.dtStart.year}-${this.dtStart.month}-${this.dtStart.day}`;
    let endDate: string = `${this.dtEnd.year}-${this.dtEnd.month}-${this.dtEnd.day}`;
    this.service.GetRemaining(startDate,endDate).subscribe((res: any) => {
      this.data = res.result;
    }, (err: any) => {
    });
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
  exportExcel(){
    let startDate: string = `${this.dtStart.year}-${this.dtStart.month}-${this.dtStart.day}`;
    let endDate: string = `${this.dtEnd.year}-${this.dtEnd.month}-${this.dtEnd.day}`;
    this.service.exportRemaining(startDate, endDate);
  }
}
