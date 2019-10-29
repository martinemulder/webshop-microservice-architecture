using HalfWerk.CommonModels.DsKlantBeheer.Models;
using System;

namespace HalfWerk.DsKlantBeheer.Test.Entities
{
    public class KlantBuilder
    {
        private long Id { get; set; }
        private string Voornaam { get; set; }
        private string Achternaam { get; set; }
        private string Telefoonnummer { get; set; }
        private string Email { get; set; }
        private Adres Adres { get; set; }

        public KlantBuilder SetId(long n) { Id = n; return this; }
        public KlantBuilder SetVoornaam(string s) { Voornaam = s; return this; }
        public KlantBuilder SetAchternaam(string s) { Achternaam = s; return this; }
        public KlantBuilder SetTelefoonnummer(string s) { Telefoonnummer = s; return this; }
        public KlantBuilder SetEmail(string s) { Email = s; return this; }
        public KlantBuilder SetAdres(Adres adr) { Adres = adr; return this; }

        public KlantBuilder SetDummy()
        {
            Id = DateTime.Now.GetHashCode();
            Voornaam = "Dummy";
            Achternaam = "van Dummy";
            Telefoonnummer = "0612345678";
            Email = "dummy@dummy.dummy";
            Adres = new Adres() {
                Id = 1,
                Huisnummer = "12",
                Land = "Nederland",
                Plaats = "Utrecht",
                Postcode = "3511 BS",
                Straatnaam = "St Jacobsstraat"
            };

            return this;
        }

        public Klant Create()
        {
            return new Klant()
            {
                Id = Id,
                Voornaam = Voornaam,
                Achternaam = Achternaam,
                Telefoonnummer = Telefoonnummer,
                Email = Email,
                Adres = Adres
            };
        }
    }
}
