using HalfWerk.CommonModels.BffWebshop.KlantBeheer;
using System;
using System.Collections.Generic;
using System.Text;

namespace HalfWerk.BffWebshop.Test.Entities
{
    public class KlantBuilder
    {
        private long Id { get; set; }
        private string Voornaam { get; set; }
        private string Achternaam { get; set; }
        private string Telefoonnummer { get; set; }
        private string Email { get; set; }
        private Adres Adres { get; set; }

        public KlantBuilder SetId(long id) { Id = id; return this; }
        public KlantBuilder SetVoornaam(string voornaam) { Voornaam = voornaam; return this; }
        public KlantBuilder SetAchternaam(string achternaam) { Achternaam = achternaam; return this; }
        public KlantBuilder SetTelefoonnummer(string telefoonnummer) { Telefoonnummer = telefoonnummer; return this; }
        public KlantBuilder SetEmail(string email) { Email = email; return this; }
        public KlantBuilder SetAdres(Adres adres) { Adres = adres; return this; }

        public KlantBuilder SetDummy()
        {
            Voornaam = "Kees";
            Achternaam = "de Koning";
            Telefoonnummer = "06-89652545";
            Email = "Kees@dekoning.nl";
            
            return this;
        }

        public Klant Create()
        {
            return new Klant()
            {
                Voornaam = Voornaam,
                Achternaam = Achternaam,
                Email = Email,
                Telefoonnummer = Telefoonnummer
            };
        }
    }
}
