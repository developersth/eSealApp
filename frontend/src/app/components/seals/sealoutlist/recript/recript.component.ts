import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { RestService } from '../../../../services/rest.service';
import { Seals } from 'app/models/seals.model';
import { SealOut } from 'app/models/seal-out.model';
import { forEach } from 'core-js/core/array';
@Component({
  selector: 'app-recript',
  templateUrl: './recript.component.html',
  styleUrls: ['./recript.component.scss'],
  providers: [RestService],
})
export class RecriptComponent implements OnInit {

  constructor(
    public activeModal: NgbActiveModal,
    private service: RestService,
    private router: Router,
  ) { }

  orderNo = '1234';
  dateTime = new Date().toLocaleString();
  @Input() truckLicense: string;
  mSealItem :any[] = [];
  pdfUrl:any;
  id: string;
  sealOutId:string;
  truckName:string;
  created:Date;
  mSealOut:SealOut[]=[];
  mSealList: Seals[] = [];
  mSealExtraList: Seals[] = [];
  sealTotal:number = 0;
  sealTotalExtra:number = 0;
  mSealOutInfoList:any[];

  ngOnInit(): void {
    //this.data = [];
    this.getData();
  }
  getData() {
    // this.service.getSealOutById(this.id).subscribe((res: any) => {
    //   this.data = res.result;
    // });
   //sealOut
   this.service.getShowReceiptHeader(this.sealOutId).subscribe((res: any) => {
    this.truckName = res.result.truckName;
    this.created =res.result.created;
    this.sealTotal =res.result.sealTotal;
    this.sealTotalExtra =res.result.sealTotalExtra;
   });
   this.getReportReceiptDetail(this.sealOutId);
   this.getSealExtraDetail(this.sealOutId);
  }
  getReportReceiptDetail(id:string){
    this.service.getReportReceiptDetail(id).subscribe(
      data => {
        this.mSealOut = data.result;
      },
      error => {
        console.log(JSON.stringify(error));
      }
    );
  }
  getSealExtraDetail(sealOutId:string){
    this.service.getSealExtraDetail(sealOutId).subscribe(
      data => {
        this.mSealExtraList = data.result;
      },
      error => {
        console.log(JSON.stringify(error));
      }
    );
  }
  getDatetimeNow() {
    return new Date();
  }
  printSlip() {


    let printWindow: Window;
    let slip = document.getElementById('slip');

    printWindow = window.open(null, "_blank", "width=600,height=450")
    let body = slip.innerHTML
    body += `<style>
    * {
        font-size: 10px;
        font-family: 'Times New Roman';
      }

      td,
      th,
      tr,
      table {
        border-top: 1px solid black;
        border-collapse: collapse;
      }

      td.description,
      th.description {
        width: 75px;
        max-width: 75px;
        text-align: center;
      }

      td.item,
      th.item {
        width: 30px;
        max-width: 30px;
        word-break: break-all;
      }

      td.pack,
      th.pack {
        width: 50px;
        max-width: 50px;
        word-break: break-all;
        text-align: center;
        align-content: center;
      }

      .centered {
        text-align: center;
        align-content: center;
      }
      .slip-header {
        font-size: 12px;
        text-align: left;
        align-content: left;
      }
      .slip {
        width: 155px;
        max-width: 155px;
      }

      img {
        display: block;
        margin-left: auto;
        margin-right: auto;
        width:40%;
        height:12%;

      }
  </style>`

    printWindow.document.write(body)
    printWindow.document.close()
    this.sleep(300).then(() => {
      if (printWindow) {
        printWindow.print();
        printWindow.close();
        this.activeModal.close();
        this.router.navigate(['/seals/sealoutlist']);

      }
    })
  }
  sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
  }
}
