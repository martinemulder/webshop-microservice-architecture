using HalfWerk.CommonModels;
using HalfWerk.CommonModels.DsBestelService;
using HalfWerk.CommonModels.DsBestelService.Models;
using HalfWerk.CommonModels.PcsBetaalService;
using HalfWerk.DsBestelService.DAL.DataMappers;
using Minor.Nijn;
using Minor.Nijn.WebScale.Attributes;
using Minor.Nijn.WebScale.Events;
using Newtonsoft.Json;

namespace HalfWerk.DsBestelService.Listeners
{
    [EventListener(NameConstants.BestelServiceEventQueue)]
    public class EventListener
    {
        private readonly IArtikelDataMapper _artikelDataMapper;
        private readonly IBestellingDataMapper _bestellingDataMapper;
        private readonly IEventPublisher _eventPublisher;

        public EventListener(IArtikelDataMapper artikelDataMapper, IBestellingDataMapper bestellingDataMapper, IEventPublisher eventPublisher)
        {
            _artikelDataMapper = artikelDataMapper;
            _bestellingDataMapper = bestellingDataMapper;
            _eventPublisher = eventPublisher;
        }

        [Topic(NameConstants.CatalogusServiceCategorieAanCatalogusToegevoegdEvent)]
        public void HandleArtikelToegevoegdEvent(EventMessage message)
        {
            Artikel artikel = JsonConvert.DeserializeObject<Artikel>(message.Message);
            _artikelDataMapper.Insert(artikel);
        }

        [Topic(NameConstants.BetaalServiceBetalingGeaccrediteerdEvent)]
        public void HandleBetalingGeaccrediteerdEvent(BestellingGeaccrediteerdEvent message)
        {
            var bestelling = _bestellingDataMapper.GetByFactuurnummer(message.Factuurnummer);

            var status = bestelling.BestelStatus;
            bestelling.BestelStatus = status < BestelStatus.Betaald && status != BestelStatus.Verzonden
                ? BestelStatus.Goedgekeurd
                : BestelStatus.Afgerond;

            _bestellingDataMapper.Update(bestelling);

            var statusBijGewerktEvent = new BestelStatusBijgewerktEvent(bestelling, NameConstants.BestelServiceBestelStatusUpgedateEvent);
            _eventPublisher.Publish(statusBijGewerktEvent);
        }
    }
}
