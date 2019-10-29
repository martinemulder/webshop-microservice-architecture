namespace HalfWerk.CommonModels.DsBestelService.Models
{
    public class BestelRegel
    {
        public long Id { get; set; }
        public long Artikelnummer { get; set; }
        public string LeverancierCode { get; set; }
        public string Naam { get; set; }
        public int Aantal { get; set; }
        public decimal PrijsExclBtw { get; set; }
        public decimal PrijsInclBtw { get; set; }
        public decimal RegelTotaalExclBtw { get; set; }
        public decimal RegelTotaalInclBtw { get; set; }
    }
}
