import { Component, Injectable, NgModule } from "@angular/core";
import { SealIn } from "app/models/seal-in.model";
import { Seals } from "app/models/seals.model";
import { Observable, of } from "rxjs";
import { RestService } from "../services/rest.service";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { type } from "os";
@Injectable()
export class Utils {
  sealIn: any[]=[];
  seal: Seals[];
  sealPack: number[] = [3, 4, 5];
  statusName :string='พร้อมใช้งาน';
  user:string=this.service.getFullNameLocalAuthen();
  constructor(
    private service: RestService
  ) {}

  calculateSealNumber(
    typePack: number, //1=3,4,5 2=3,3=4,5=5
    sealTotal: number,
    sealStartNo: number
  ): Observable<any> {
    //reset values

    let total: number = sealTotal;
    let currentEnd: number = sealStartNo + total;
    let currentNumber: number = sealStartNo;
    let currentSize: number = 0;
    let sealNo: string;
    let sealBetween: string;

    this.sealIn = [];
    for (let i = 0; i < total; i++) {
     this.seal = [];
      //pack 3
      if (typePack === 3) {
        currentSize = 3;
        console.log(total%currentSize);
        sealBetween = `${currentNumber}-${currentNumber + currentSize - 1}`;
        //console.log(typePack);
        if (currentNumber + currentSize <= currentEnd) {
          for (let index = 0; index < currentSize; index++) {
            this.seal.push({
              id: 0,
              sealNo: (currentNumber + index).toString(),
              type: 1,
              typeName:'ปกติ',
              status: 1,
              statusName:'ยังไม่ได้ใช้งาน',
              createdBy: this.user,
              updatedBy: this.user,
            });
          }
          this.sealIn.push({
            sealBetween: sealBetween,
            sealList: this.seal,
            pack: currentSize,
            isActive: false,
            createdBy: this.user,
            updatedBy: this.user,
          });
        } else if (currentEnd - currentNumber === 2) {
          for (let index = 0; index < 2; index++) {
            this.seal.push({
              id: 0,
              sealNo: (currentNumber + index).toString(),
              type: 1,
              typeName:'ปกติ',
              status: 1,
              statusName:'ยังไม่ได้ใช้งาน',
              createdBy: this.user,
              updatedBy: this.user,
            });
          }
          this.sealIn.push({
            sealBetween: currentNumber.toString(),
            sealList: this.seal,
            pack: 1,
            isActive: false,
            createdBy: this.user,
            updatedBy: this.user,
          });
          this.sealIn.push({
            sealBetween: (currentNumber + 1).toString(),
            sealList: this.seal,
            pack: 1,
            isActive: false,
            createdBy: this.user,
            updatedBy: this.user,
          });
        } else if (currentEnd - currentNumber === 1) {
          this.seal.push({
            id: 0,
            sealNo: currentNumber.toString(),
            type: 1,
            typeName:'ปกติ',
            status: 1,
            statusName:'ยังไม่ได้ใช้งาน',
            createdBy: this.user,
            updatedBy: this.user,
          });
          this.sealIn.push({
            sealBetween: currentNumber.toString(),
            sealList: this.seal,
            pack: 1,
            isActive: false,
            createdBy: this.user,
            updatedBy: this.user,
          });
        }
      }
      //pack4
      else if (typePack === 4) {
        currentSize = 4;
        sealBetween = `${currentNumber}-${currentNumber + currentSize - 1}`;
        console.log(typePack);
        if (currentNumber + currentSize <= currentEnd) {
          for (let index = 0; index < currentSize; index++) {
            this.seal.push({
              id: 0,
              sealNo: (currentNumber + index).toString(),
              type: 1,
              typeName:'ปกติ',
              status: 1,
              statusName:'ยังไม่ได้ใช้งาน',
              createdBy: this.user,
              updatedBy: this.user,
            });
          }
          this.sealIn.push({
            sealBetween: sealBetween,
            sealList: this.seal,
            pack: currentSize,
            isActive: false,
            createdBy: this.user,
            updatedBy: this.user,
          });
        } else if (currentEnd - currentNumber === 2) {
          for (let index = 0; index < 2; index++) {
            this.seal.push({
              id: 0,
              sealNo: (currentNumber + index).toString(),
              type: 1,
              typeName:'ปกติ',
              status: 1,
              statusName:'ยังไม่ได้ใช้งาน',
              createdBy: this.user,
              updatedBy: this.user,
            });
          }
          this.sealIn.push({
            sealBetween: currentNumber.toString(),
            sealList: this.seal,
            pack: 1,
            isActive: false,
            createdBy: this.user,
            updatedBy: this.user,
          });
          this.sealIn.push({
            sealBetween: (currentNumber + 1).toString(),
            sealList: this.seal,
            pack: 1,
            isActive: false,
            createdBy: this.user,
            updatedBy: this.user,
          });
        } else if (currentEnd - currentNumber === 1) {
          this.seal.push({
            id: 0,
            sealNo: currentNumber.toString(),
            type: 1,
            typeName:'ปกติ',
            status: 1,
            statusName:'ยังไม่ได้ใช้งาน',
            createdBy: this.user,
            updatedBy: this.user,
          });
          this.sealIn.push({
            sealBetween: currentNumber.toString(),
            sealList: this.seal,
            pack: 1,
            isActive: false,
            createdBy: this.user,
            updatedBy: this.user,
          });
        }
      }
      //pack5
      else if (typePack === 5) {
        currentSize = 5;
        sealBetween = `${currentNumber}-${currentNumber + currentSize - 1}`;
        console.log(typePack);
        if (currentNumber + currentSize <= currentEnd) {
          for (let index = 0; index < currentSize; index++) {
            this.seal.push({
              id: 0,
              sealNo: (currentNumber + index).toString(),
              type: 1,
              typeName:'ปกติ',
              status: 1,
              statusName:'ยังไม่ได้ใช้งาน',
              createdBy: this.user,
              updatedBy: this.user,
            });
          }
          this.sealIn.push({
            sealBetween: sealBetween,
            sealList: this.seal,
            pack: currentSize,
            isActive: false,
            createdBy: this.user,
            updatedBy: this.user,
          });
        } else if (currentEnd - currentNumber === 2) {
          for (let index = 0; index < 2; index++) {
            this.seal.push({
              id: 0,
              sealNo: (currentNumber + index).toString(),
              type: 1,
              typeName:'ปกติ',
              status: 1,
              statusName:'ยังไม่ได้ใช้งาน',
              createdBy: this.user,
              updatedBy: this.user,
            });
          }
          this.sealIn.push({
            sealBetween: currentNumber.toString(),
            sealList: this.seal,
            pack: 1,
            isActive: false,
            createdBy: this.user,
            updatedBy: this.user,
          });
          this.sealIn.push({
            sealBetween: (currentNumber + 1).toString(),
            sealList: this.seal,
            pack: 1,
            isActive: false,
            createdBy: this.user,
            updatedBy: this.user,
          });
        } else if (currentEnd - currentNumber === 1) {
          this.seal.push({
            id: 0,
            sealNo: currentNumber.toString(),
            type: 1,
            typeName:'ปกติ',
            status: 1,
            statusName:'ยังไม่ได้ใช้งาน',
            createdBy: this.user,
            updatedBy: this.user,
          });
          this.sealIn.push({
            sealBetween: currentNumber.toString(),
            sealList: this.seal,
            pack: 1,
            isActive: false,
            createdBy: this.user,
            updatedBy: this.user,
          });
        }
      }
      //pack 3 4 5
      else {
        currentSize = this.sealPack[i % this.sealPack.length];
        sealBetween = `${currentNumber}-${currentNumber + currentSize - 1}`;

        if (currentNumber + currentSize <= currentEnd) {
          for (let index = 0; index < currentSize; index++) {
            this.seal.push({
              id: 0,
              sealNo: (currentNumber + index).toString(),
              type: 1,
              typeName:'ปกติ',
              status: 1,
              statusName:'ยังไม่ได้ใช้งาน',
              createdBy: this.user,
              updatedBy: this.user,
            });
          }
          this.sealIn.push({
            sealBetween: sealBetween,
            sealList: this.seal,
            pack: currentSize,
            isActive: false,
            createdBy: this.user,
            updatedBy: this.user,
          });
        
        } else if (currentEnd - currentNumber === 1) {
          sealBetween = `${currentNumber}`;
          this.seal.push({
            id: 0,
            sealNo: currentNumber.toString(),
            type: 1,
            typeName:'ปกติ',
            status: 1,
            statusName:'ยังไม่ได้ใช้งาน',
            createdBy: this.user,
            updatedBy: this.user,
          });
          this.sealIn.push({
            sealBetween: sealBetween,
            sealList: this.seal,
            pack: 1,
            isActive: false,
          });
        } else if (currentEnd - currentNumber === 2) {
          for (let index = 0; index < 2; index++) {
            this.seal.push({
              id: 0,
              sealNo: (currentNumber + index).toString(),
              type: 1,
              typeName:'ปกติ',
              status: 1,
              statusName:'ยังไม่ได้ใช้งาน',
              createdBy: this.user,
              updatedBy: this.user,
            });
          }
          this.sealIn.push({
            sealBetween: currentNumber.toString(),
            sealList: this.seal,
            pack: 1,
            isActive: false,
            createdBy: this.user,
            updatedBy: this.user,
          });
          this.sealIn.push({
            sealBetween: (currentNumber + 1).toString(),
            sealList: this.seal,
            pack: 1,
            isActive: false,
            createdBy: this.user,
            updatedBy: this.user,
          });
        } else if (currentNumber + currentSize > currentEnd) {
          this.sealPack.forEach((pack: number) => {
            if (currentNumber + pack <= currentEnd) {
              sealBetween = `${currentNumber}-${currentNumber + pack - 1}`;
              this.sealIn.push({
                sealBetween: sealBetween,
                sealList: this.seal,
                pack: pack,
                isActive: false,
                createdBy: this.user,
                updatedBy: this.user,
              });
            }
          });
        }
      }
      currentNumber += currentSize;
    }
    //delay(200);
    return of(this.sealIn);
  }
}