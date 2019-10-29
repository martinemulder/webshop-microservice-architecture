using HalfWerk.CommonModels.BffWebshop.KlantBeheer;
using System;
using System.Collections.Generic;
using System.Text;

namespace HalfWerk.BffWebshop.Test.Entities
{
    public class AdresBuilder
    {
        private long Id { get; set; }
        private string Straatnaam { get; set; }
        private string Postcode { get; set; }
        private string Huisnummer { get; set; }
        private string Plaats { get; set; }
        private string Land { get; set; }

        private long KlantId { get; set; }
        private Klant Klant { get; set; }

        public AdresBuilder SetId(long id) { Id = id; return this; }
        public AdresBuilder SetStraatnaam(string straatnaam) { Straatnaam = straatnaam; return this; }
        public AdresBuilder SetPostcode(string postcode) { Postcode = postcode; return this; }
        public AdresBuilder SetHuisnummer(string huisnummer) { Huisnummer = huisnummer; return this; }
        public AdresBuilder SetPlaats(string plaats) { Plaats = plaats; return this; }
        public AdresBuilder SetLand(string land) { Land = land; return this; }
        public AdresBuilder SetDummy()
        {
            Straatnaam = "Oranjestraat";
            Huisnummer = "51";
            Plaats = "Utrecht";
            Postcode = "5245TY";
            Land = "Nederland";

            return this;
        }
        public Adres Create()
        {
            return new Adres()
            {
                Land = Land,
                Postcode = Postcode,
                Plaats = Plaats,
                Huisnummer = Huisnummer,
                Straatnaam = Straatnaam
            };
        }
        
    }
}
