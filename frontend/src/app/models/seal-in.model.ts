import { th } from "date-fns/locale";
import { DatePipe } from "@angular/common";
export class SealIn {
  public id: number;
  public sealInId: string;
  public sealBetween: string;
  public sealList: string;  //json string
  public pack: number;
  public isUsed: boolean;
  public isActive: boolean;
  public checked: boolean;
  public createdBy: string;
  public updatedBy: string;
  public created: Date;
  public updated: Date;
  constructor() {
    this.sealBetween = '';
    this.sealList = '';
    this.pack = 0;
    this.isUsed = false;
    this.checked = false;
  }
}



