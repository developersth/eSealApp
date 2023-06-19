import { Injectable } from "@angular/core";

import { Observable, of, forkJoin } from "rxjs";
import { delay, tap } from "rxjs/operators";
import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import { environment } from "environments/environment";
import { forEach } from "core-js/core/array";

//----------------------------------------------------------------
import { User } from "../models/user.model";
import { Roles } from "app/models/roles.model";
import { Truck } from "../models/truck.model";
import { Response } from "../models/response.model";
import { SealTypes } from "app/models/seal-types.model";
import { SealStatus } from "app/models/seal-status.model";


@Injectable()
export class RestService {
  constructor(private http: HttpClient) {}
  private readonly apiUrl = `${environment.apiUrl}`;
  private headers = new HttpHeaders({ 'Content-Type': 'application/json' });
  private httpOptions = {
    headers: new HttpHeaders({ "Content-Type": "application/pdf" }),
    responseType: "arraybuffer" as "json",
  };

  //----------------------------------------------------------------
  private sealInUrl = `${this.apiUrl}/sealin`;
  private sealUrl = `${this.apiUrl}/Seal`;
  private sealOutUrl = `${this.apiUrl}/sealout`;
  private userUrl = `${this.apiUrl}/user`;
  private truckUrl = `${this.apiUrl}/truck`;
  private rolesUrl = `${this.apiUrl}/roles`;
  private reportUrl = `${this.apiUrl}/report`;

  //----------------------------------------------------------------
  getFullNameLocalAuthen() {
    return localStorage.getItem(environment.fullNameLocalAuthen);
  }
  getToken() {
    return localStorage.getItem(environment.keyLocalAuthenInfo);
  }
  //----------------------------------------------------------------
  getRoles():Observable<Roles[]>{
    return this.http.get<Roles[]>(`${this.rolesUrl}/GetRoles`);
  }
  //----------------------------------------------------------------
  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${this.userUrl}/GetUser`);
  }
  getUser(id: number): Observable<User> {
    return this.http.get<User>(`${this.userUrl}/${id}`);
  }
  createUser(body: any): Observable<User> {
    return this.http.post<User>(`${this.userUrl}/register`, body);
  }
  updateUser(id: string, body: any): Observable<User> {
    return this.http.put<User>(`${this.userUrl}/${id}`, body);
  }
  deleteUser(id: string): Observable<any> {
    return this.http.delete(`${this.userUrl}/${id}`);
  }
  //----------------------------------------------------------------
  getTruck(): Observable<Truck[]> {
    return this.http.get<Truck[]>(this.truckUrl);
  }
  getTruckById(id: string): Observable<Truck[]> {
    return this.http.get<Truck[]>(`${this.truckUrl}/${id}`);
  }
  addTruck(truck: Truck): Observable<Response> {
    return this.http.post<Response>(`${this.truckUrl}`, truck);
  }
  updateTruck(id: string, truck: Truck): Observable<Truck> {
    return this.http.put<Truck>(`${this.truckUrl}/${id}`, truck);
  }
  deleteTruck(id: string): Observable<any> {
    return this.http.delete(`${this.truckUrl}/${id}`);
  }
  //----------------------------------------------------------------
  addSealIn(items: any): Observable<any> {
    let item = JSON.stringify(items);
    return this.http.post<any>(`${this.sealInUrl}`, item, { headers:this.headers });
  }
  getSealIn(
    isActive: string,
    columnSearch: string,
    searchTerm: string,
    startDate: string,
    endDate: string
  ): Observable<any> {
    return this.http.get<any[]>(
      `${this.sealInUrl}?pIsActive=${isActive}&pColumnSearch=${columnSearch}&searchTerm=${searchTerm}&pStartDate=${startDate}&pEndDate=${endDate}`,
      { headers:this.headers }
    );
  }
  getSeaBetWeen(): Observable<any> {
    return this.http.get<any[]>(`${this.sealInUrl}/GetSealBetWeen`, {
      headers:this.headers,
    });
  }
  deleteSealIn(id: string): Observable<any> {
    return this.http.delete<any[]>(`${this.sealInUrl}/${id}`, { headers:this.headers });
  }
  deleteSealAll(itemId: string[]): Observable<any> {
    const deleteRequests = itemId.map((itemId) => this.deleteSealIn(itemId));
    return forkJoin(deleteRequests);
  }
  //----------------------------------------------------------------
  getTypes():Observable<SealTypes[]>{
    return this.http.get<SealTypes[]>(`${this.sealUrl}/GetTypes`);
  }

  getStatus():Observable<SealStatus[]>{
    return this.http.get<SealStatus[]>(`${this.sealUrl}/GetStatus`);
  }
  getSeals( startDate: string,endDate: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.sealUrl}?pStartDate=${startDate}&pEndDate=${endDate}`);
  }
  getSealItemBySealInId(id: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.sealUrl}/BySealInId/${id}`, {
      headers:this.headers,
    });
  }
  getSealExtra(): Observable<any[]> {
    return this.http.get<any[]>(`${this.sealUrl}/GetSealExtra`,{headers:this.headers});
  }
  getSealExtraById(id:number): Observable<any[]> {
    return this.http.get<any[]>(`${this.sealUrl}/GetSealExtra/${id}`,{headers:this.headers});
  }
  addSeal(items: any): Observable<any> {
    let item = JSON.stringify(items);
    return this.http.post<any>(`${this.sealUrl}`, item, { headers:this.headers });
  }
  updateSeal(id:number,items: any): Observable<any> {
    let item = JSON.stringify(items);
    return this.http.put<any>(`${this.sealUrl}/${id}`, item, { headers:this.headers });
  }
  deleteSeal(id: string): Observable<any> {
    return this.http.delete<any[]>(`${this.sealUrl}/${id}`, { headers:this.headers });
  }
  //----------------------------------------------------------------
  deleteSealOut(id: string): Observable<any> {
    return this.http.delete<any[]>(`${this.sealOutUrl}/${id}`, { headers:this.headers });
  }
  addSealOut(item: any): Observable<any> {
    return this.http.post<any>(`${this.sealOutUrl}`, item, {
      headers:this.headers,
    });
  }
  updateSealOut(id: string, item: any): Observable<any> {
    return this.http.put<any>(`${this.sealOutUrl}/${id}`, item, {
      headers:this.headers,
    });
  }
  getSealOutById(id: string): Observable<any> {
    return this.http.get<any[]>(`${this.sealOutUrl}/${id}`, { headers:this.headers });
  }
  getSealOutInfoList(id: string): Observable<any> {
    return this.http.get<any[]>(`${this.sealOutUrl}/GetSealOutInfoList/${id}`, { headers:this.headers });
  }
  getReportReceiptDetail(id: string): Observable<any> {
    return this.http.get<any[]>(`${this.sealOutUrl}/ShowReceiptDetail/${id}`, { headers:this.headers });
  }
  getReportReceipt(id: string): Observable<any> {
    return this.http.get<any[]>(`${this.sealOutUrl}/showreceipt/${id}`, { headers:this.headers });
  }
  getSealOutInfo(id: string): Observable<any> {
    return this.http.get<any[]>(`${this.sealOutUrl}/GetSealOutInfo/${id}`, { headers:this.headers });
  }
  getSealChange(): Observable<any> {
    return this.http.get<any[]>(`${this.sealUrl}/GetSealChange`, { headers:this.headers });
  }
  getSealStatus(): Observable<any> {
    return this.http.get<any[]>(`${this.sealUrl}/GetSealStatus`, { headers:this.headers });
  }
  sealChange(items: any): Observable<any> {
    let item = JSON.stringify(items);
    console.log(item);
    return this.http.post<any>(`${this.sealOutUrl}/SealChange`, item, {
      headers:this.headers,
    });
  }
  getSealOutAll(
    isCancel: string,
    columnSearch: string,
    searchTerm: string,
    startDate: string,
    endDate: string
  ): Observable<any> {
    const body = { startDate: startDate, endDate: endDate };
    return this.http.get<any[]>(
      `${this.sealOutUrl}?pIsActive=${isCancel}&pColumnSearch=${columnSearch}&searchTerm=${searchTerm}&pStartDate=${startDate}&pEndDate=${endDate}`,
      {
        headers:this.headers,
      }
    );
  }
  //---------------------------------------------------------------- report
  GetRemaining( startDate: string,endDate: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.reportUrl}/GetRemaining?pStartDate=${startDate}&pEndDate=${endDate}`);
  }
  getSealChanges(startDate: string,endDate: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.reportUrl}/GetSealChanges?pStartDate=${startDate}&pEndDate=${endDate}`);
  }
  exportSealChanges(startDate: string,endDate: string) {
    let apiEndpoint = `${this.reportUrl}/ExportSealChanges?pStartDate=${startDate}&pEndDate=${endDate}`;
    this.http.get(apiEndpoint, { responseType: 'blob' }).subscribe((response: any) => {
      const blob = new Blob([response], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });

      // สร้างลิงก์ดาวน์โหลด
      const link = document.createElement('a');
      link.href = window.URL.createObjectURL(blob);
      let new_date: Date = new Date();
      link.download = `report_SealChange_${new_date.getFullYear()}${new_date.getMonth()}${new_date.getDate()}${new_date.getTime()}.xlsx`; // ใช้ชื่อไฟล์ที่ได้รับจากเซิร์ฟเวอร์ หากไม่สามารถรับชื่อไฟล์ได้ให้ใช้ชื่อเริ่มต้นเป็น "SealChanges.xlsx"
      link.click();
    });
  }
  exportRemaining(startDate: string,endDate: string) {
    let apiEndpoint = `${this.reportUrl}/ExportRemaining?pStartDate=${startDate}&pEndDate=${endDate}`;
    this.http.get(apiEndpoint, { responseType: 'blob' }).subscribe((response: any) => {
      const blob = new Blob([response], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });

      // สร้างลิงก์ดาวน์โหลด
      const link = document.createElement('a');
      link.href = window.URL.createObjectURL(blob);
      let new_date: Date = new Date();
      link.download = `report_SealRemaining_${new_date.getFullYear()}${new_date.getMonth()}${new_date.getDate()}${new_date.getTime()}.xlsx`; // ใช้ชื่อไฟล์ที่ได้รับจากเซิร์ฟเวอร์ หากไม่สามารถรับชื่อไฟล์ได้ให้ใช้ชื่อเริ่มต้นเป็น "SealChanges.xlsx"
      link.click();
    });
  }
}
