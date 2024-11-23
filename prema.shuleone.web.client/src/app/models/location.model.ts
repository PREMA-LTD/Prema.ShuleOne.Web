export interface BaseType {
    id: number;
    name: string;
  }
  
  export interface County extends BaseType {}
  
  export interface Ward extends BaseType {}
  
  export interface Subcounty extends BaseType {}
  
  export interface LocationData {
    countyId: number;
    subcountyId: number;
  }
  