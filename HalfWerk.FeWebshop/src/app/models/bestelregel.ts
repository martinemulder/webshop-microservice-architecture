export class BestelRegel {
  id: number;
  afbeeldingUrl: string;
  artikelnummer: number;
  leverancierCode?: string;
  naam: string;
  aantal: number;
  prijs?: number;
  prijsExclBtw?: number;
  prijsInclBtw?: number;
  regelTotaalExclBtw?: number;
  regelTotaalInclBtw?: number;
}
