import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { KeycloakService } from 'keycloak-angular';
import { KeycloakProfile } from 'keycloak-js';
import { County, LocationData, Subcounty, Ward } from 'app/models/location.model';


@Injectable({
  providedIn: 'root'
})

export class LocationService {
  private readonly keycloakService = inject(KeycloakService);
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  async getCounties(): Promise<Observable<County[]>> {
    return this.http.get<County[]>(`${this.apiUrl}/Location/County`);
  }
  
  async getSubcounties(countyId: number): Promise<Observable<Subcounty[]>> {
    return this.http.get<Subcounty[]>(`${this.apiUrl}/Location/Subcounty/${countyId}`);
  }

  async getWards(subcountyId?: number): Promise<Observable<Ward[]>> {
    return this.http.get<Ward[]>(`${this.apiUrl}/Location/Ward/${subcountyId}`);
  }

  async getLocation(wardId: number): Promise<Observable<LocationData>> {
    return this.http.get<LocationData>(`${this.apiUrl}/Location/${wardId}`);
  }

}
