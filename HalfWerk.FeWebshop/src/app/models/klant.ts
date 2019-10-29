import { Adres } from './adres';

export class Klant {
  id?: number;
  voornaam: string;
  achternaam: string;
  telefoonnummer: string;
  email: string;
  adres: Adres;
}
