import { Bestelling } from '@models/bestelling';
import { TotalOrderPricePipe } from './total-order-price.pipe';

describe('TotalNumberProductsOrderPipe', () => {
  let totalOrderPricePipe: TotalOrderPricePipe;
  let bestelling: Bestelling;

  beforeEach(() => {
    totalOrderPricePipe = new TotalOrderPricePipe();

    bestelling = new Bestelling();
    bestelling.bestelRegels = [
      { id: 0, artikelnummer: 1, leverancierCode: '45454', naam: 'Leverancier', aantal: 3, prijsExclBtw: 19.99, afbeeldingUrl: '' },
      { id: 0, artikelnummer: 2, leverancierCode: '45454', naam: 'Leverancier', aantal: 1, prijsExclBtw: 21.99, afbeeldingUrl: '' }
    ];
  });

  it('should return 99.18', () => {
    expect(totalOrderPricePipe.transform(bestelling)).toBe(99.18);
  });

});
