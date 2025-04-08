import { Injectable } from '@angular/core';
import { HousingLocationType } from './housinglocation.type';

@Injectable({
  providedIn: 'root',
})
export class HousingService {
  private url = 'https://localhost:7117/api/Locations'; 

  public async getAllHousingLocations(): Promise<HousingLocationType[]> {
    const data = await fetch(this.url);
    return (await data.json()) ?? [];
  }

  public async getHousingLocationById( id: number ): Promise<HousingLocationType | undefined> {
    const datum = await fetch(`${this.url}/${id}`);
    return (await datum.json()) ?? {};
  }

  public submitApplication(firstName: string, lastName: string, email: string) {
    console.log(
      `Homes application received: firstName: ${firstName}, lastName: ${lastName}, email: ${email}.`
    );
  }
}
