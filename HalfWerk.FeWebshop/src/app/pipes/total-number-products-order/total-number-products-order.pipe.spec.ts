import { TotalNumberProductsOrderPipe } from './total-number-products-order.pipe';
import { Bestelling } from '@models/bestelling';

describe('TotalNumberProductsOrderPipe', () => {
  let totalNumberProductsOrderPipe: TotalNumberProductsOrderPipe;
  let bestelling: Bestelling;

  beforeEach(() => {
    totalNumberProductsOrderPipe = new TotalNumberProductsOrderPipe();

    bestelling = new Bestelling();
    bestelling.bestelRegels = [
      { id: 0, artikelnummer: 1, leverancierCode: '45454', naam: 'Leverancier', aantal: 3, prijsExclBtw: 19.99, afbeeldingUrl: '' },
      { id: 0, artikelnummer: 2, leverancierCode: '45454', naam: 'Leverancier', aantal: 1, prijsExclBtw: 19.99, afbeeldingUrl: '' }
    ];
  });

  it('should return 4', () => {
    expect(totalNumberProductsOrderPipe.transform(bestelling)).toBe(4);
  });

});
