export interface User {
  id: number;
  username: string;
  password: string;
  name: string;
  email: string;
  isActive:boolean;
  role:{
    name:string
  }
}
