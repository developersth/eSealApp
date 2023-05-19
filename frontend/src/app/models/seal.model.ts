import { th } from "date-fns/locale";
import { DatePipe } from "@angular/common";
export class Seal {
  public _id: string;
  public id: { increment: number };
  public sealNoItem: {
    some(arg0: (seal: any) => any): unknown; sealNo: string;
  }
  public sealBetween: string;
  public pack: number;
  public isUsed: boolean;
  public checked: boolean;
  public createAt: Date;
  public createAtSt: string;
  constructor() {
    this.id.increment = 0;
    this.sealBetween = '';
    this.pack = 0;
    this.isUsed = false;
    this.checked = false;
  }
}

export class SealOut {
  public id: number;
  public sealTotal: number;
  public sealTotalExtra: number;
  public truckId: number;
  public truckName: string;
  public sealItemList:string;
  public sealList:any[];
  public sealItemListExtra: string;
  public created: Date;
  public isCancel:boolean;
  public checked:boolean;
  public sealType:number;
  public sealTypeName:string;
  constructor() {
     this.sealTotal= 0;
     this.sealTotalExtra= 0;
     this.truckId= 0;
     this.truckName= 'x';
     this.sealItemList='';
     this.sealItemListExtra= '';
     this.checked=false;
     this.isCancel=false;
     this.sealType=1;
     this.sealTypeName='ปกติ'
  }
}
export class SealOutInfo {
  public id:number;
  public sealOutId:number;
  public sealInId:number;
  public sealBetween:string;
  public pack:number;
  public sealType:number;
  public sealTypeName:string;
}
export class SealItemList {
  id: number;
  sealBetween: string;
  pack: number;
  sealType: number;
  sealTypeName: string;
  sealItem:any;
}
export class SealItem {
  id: number;
  sealNo: string;
  type: number;
  status: number;
  isUsd: string;
}
