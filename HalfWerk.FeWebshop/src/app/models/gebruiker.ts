import { Klant } from './klant';

export class Gebruiker {
  id?: number;
  email: string;
  password: string;
  token?: string;
  klant?: Klant;
}
