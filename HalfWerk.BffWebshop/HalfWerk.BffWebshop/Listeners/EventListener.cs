using System;
using AutoMapper;
using HalfWerk.BffWebshop.DataMapper;
using HalfWerk.BffWebshop.Entities;
using HalfWerk.CommonModels;
using HalfWerk.CommonModels.BffWebshop;
using HalfWerk.CommonModels.BffWebshop.BestellingService;
using HalfWerk.CommonModels.BffWebshop.KlantBeheer;
using HalfWerk.CommonModels.DsBestelService;
using HalfWerk.CommonModels.DsKlantBeheer;
using Minor.Nijn;
using Minor.Nijn.WebScale.Attributes;
using Newtonsoft.Json;
using Artikel = HalfWerk.CommonModels.BffWebshop.Artikel;
using Bestelling = HalfWerk.CommonModels.DsBestelService.Models.Bestelling;

namespace HalfWerk.BffWebshop.Listeners
{
    [EventListener(NameConstants.BffWebshopEventQueue)]
    public class EventListener
    {
        private readonly IArtikelDataMapper _artikelDataMapper;
        private readonly IKlantDataMapper _klantDataMapper;
        private readonly IBestellingDataMapper _bestellingDataMapper;

        public EventListener(IArtikelDataMapper artikelDataMapper, IKlantDataMapper klantDataMapper, IBestellingDataMapper bestellingDataMapper)
        {
            _artikelDataMapper = artikelDataMapper;
            _klantDataMapper = klantDataMapper;
            _bestellingDataMapper = bestellingDataMapper;
        }

        [Topic(NameConstants.CatalogusServiceCategorieAanCatalogusToegevoegdEvent)]
        public void ReceiveAddArtikelToCatalogus(EventMessage message)
        {
            Artikel artikel = JsonConvert.DeserializeObject<Artikel>(message.Message);
            var artikelEntity = new ArtikelEntity()
            {
                Artikelnummer = artikel.Artikelnummer,
                Naam = artikel.Naam,
                Beschrijving = artikel.Beschrijving,
                Prijs = artikel.Prijs,
                AfbeeldingUrl = artikel.AfbeeldingUrl,
                LeverbaarVanaf = artikel.LeverbaarVanaf,
                LeverbaarTot = artikel.LeverbaarTot,
                Leveranciercode = artikel.Leveranciercode,
                Leverancier = artikel.Leverancier,
            };

            foreach (var cat in artikel.Categorieen)
            {
                artikelEntity.ArtikelCategorieen.Add(new ArtikelCategorieEntity()
                {
                    Artikel = artikelEntity,
                    Categorie = new CategorieEntity()
                    {
                        Categorie = cat
                    }
                });
            }

            _artikelDataMapper.Insert(artikelEntity);
        }

        [Topic(
            NameConstants.MagazijnServiceVoorraadVerhoogdEvent, 
            NameConstants.MagazijnServiceVoorraadVerlaagdEvent
        )]
        public void HandleVoorraadChanged(EventMessage message)
        {
            var voorraad = JsonConvert.DeserializeObject<Voorraad>(message.Message);
            var artikel = _artikelDataMapper.GetById(voorraad.Artikelnummer);

            artikel.Voorrraad = voorraad.NieuweVoorraad > Constants.MaxVoorraad
                ? Constants.MaxVoorraad
                : voorraad.NieuweVoorraad;

            _artikelDataMapper.Update(artikel);
        }

        [Topic(NameConstants.BestelServiceBestellingGeplaatstEvent)]
        public void ReceiveBestellingGeplaatstEvent(BestellingGeplaatstEvent message)
        {
            var bestelling = Mapper.Map<BevestigdeBestelling>(message.Bestelling);

            int index = 0;
            foreach(var bestelRegel in message.Bestelling.BestelRegels)
            {
                var artikel = _artikelDataMapper.GetById((int)bestelRegel.Artikelnummer);
                bestelling.BestelRegels[index].AfbeeldingUrl = artikel.AfbeeldingUrl;
                index++;
            }
            _bestellingDataMapper.Insert(bestelling);
        }

        [Topic(NameConstants.KlantBeheerKlantToegevoegdEvent)]
        public void ReceiveKlantToegevoegdEvent(KlantToegevoegdEvent klantEvent)
        {
            var klant = Mapper.Map<Klant>(klantEvent.Klant);

            _klantDataMapper.Insert(klant);
        }

        [Topic(NameConstants.BestelServiceBestelStatusUpgedateEvent)]
        public void ReceiveBestelStatusBijgewerktEvent(BestelStatusBijgewerktEvent bestelStatusBijgewerktEvent)
        {
            var bestelling = _bestellingDataMapper.GetById(bestelStatusBijgewerktEvent.Bestelling.Id);
            
            bestelling.BestelStatus = (BestelStatus) Enum.ToObject(typeof(BestelStatus), (int) bestelStatusBijgewerktEvent.Bestelling.BestelStatus);

            _bestellingDataMapper.Update(bestelling);
        }
    }
}
