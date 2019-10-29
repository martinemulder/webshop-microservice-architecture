import { VoorraadPipe } from './voorraad.pipe';

describe('VoorraadPipe', () => {
  let voorraadPipe: VoorraadPipe;

  beforeEach(() => {
    voorraadPipe = new VoorraadPipe();
  });

  it('should return 0 op voorraad', () => {
    expect(voorraadPipe.transform(0)).toBe('0 op voorraad');
  });

  it('should return 8 op voorraad', () => {
    expect(voorraadPipe.transform(8)).toBe('8 op voorraad');
  });

  it('should return 8 op voorraad if more than 8', () => {
    expect(voorraadPipe.transform(9)).toBe('8 op voorraad');
  });
});
