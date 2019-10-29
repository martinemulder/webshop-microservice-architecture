namespace HalfWerk.BffWebshop.Entities
{
    public class MagazijnSessionEntity : IEntity<long>
    {
        public long Id { get; set; }
        public string MedewerkerEmail { get; set; }
        public long Factuurnummer { get; set; }

        public long GetKeyValue()
        {
            return Id;
        }
    }
}