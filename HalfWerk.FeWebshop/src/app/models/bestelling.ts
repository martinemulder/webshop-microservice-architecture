import { ContactInfo } from './contactInfo';
import { Afleveradres } from './afleveradres';
import { BestelRegel } from './bestelregel';
import { BestelStatus } from '@models/bestelStatus';

export class Bestelling {
  constructor() {}

  id: number;
  factuurnummer: number;
  klantnummer: number;
  contactInfo: ContactInfo;
  afleverAdres: Afleveradres;
  bestelRegels: BestelRegel[];
  besteldatum: string;
  bestelStatus: BestelStatus;
  factuurTotaalExclBtw: number;
  factuurTotaalInclBtw: number;
}
