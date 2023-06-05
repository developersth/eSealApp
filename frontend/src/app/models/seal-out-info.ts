export class SealOutInfo {
  public id: number;
  public sealOutId: string;
  public sealInId: string;
  public sealTotal: number;
  public sealTotalExtra: number;
  public sealList:string;
  public sealExtraList:string;
  public truckId: number;
  public truckName: string;
  public driverId: number;
  public driverName: string;
  public isCancel:boolean;
  public createdBy: string;
  public UpdatedBy: string;
  public Created:Date;
  public Updated:Date;
  public checked: boolean;
  constructor() {
     this.sealTotal= 0;
     this.sealTotalExtra= 0;
     this.truckId= 0;
     this.truckName= 'x';
     this.sealList='';
     this.sealExtraList='';
     this.checked=false;
     this.isCancel=false;
  }
}
