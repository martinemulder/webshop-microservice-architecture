using HalfWerk.CommonModels.DsKlantBeheer.Models;
using System.Collections.Generic;

namespace HalfWerk.CommonModels.DummyData
{
    public static class DummyKlanten
    {
        public static readonly List<Klant> Klanten = new List<Klant>
        {
            new Klant
            {
                Voornaam = "klant",
                Achternaam = "klant",
                Email = "klant",
                Telefoonnummer = "06123456789",
                Adres = new Adres
                {
                    Straatnaam = "Straatnaam",
                    Huisnummer = "10A",
                    Postcode = "1234 AB",
                    Plaats = "Plaats",
                    Land = "Land"
                }
            },
            new Klant
            {
                Voornaam = "holygrail",
                Achternaam = "holygrail",
                Email = "holygrail",
                Telefoonnummer = "06123456789",
                Adres = new Adres
                {
                    Straatnaam = "Straatnaam",
                    Huisnummer = "10A",
                    Postcode = "1234 AB",
                    Plaats = "Plaats",
                    Land = "Land"
                }
            }
        };
    }
}
