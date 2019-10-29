using AutoMapper;
using HalfWerk.CommonModels.BffWebshop.BestellingService;
using HalfWerk.CommonModels.DsBestelService.Models;
using HalfWerk.CommonModels.DsKlantBeheer.Models;
using Afleveradres = HalfWerk.CommonModels.BffWebshop.BestellingService.Afleveradres;
using Bestelling = HalfWerk.CommonModels.DsBestelService.Models.Bestelling;
using BestelRegel = HalfWerk.CommonModels.DsBestelService.Models.BestelRegel;
using ContactInfo = HalfWerk.CommonModels.BffWebshop.BestellingService.ContactInfo;

namespace HalfWerk.BffWebshop
{
    public static class ConfigAutoMapper
    {
        private static bool _configured;
        public static void Initialize()
        {
            if (_configured)
            {
                return;
            }

            Mapper.Initialize(conf =>
            {
                conf.CreateMap<CommonModels.BffWebshop.KlantBeheer.Klant, Klant>();
                conf.CreateMap<CommonModels.BffWebshop.BestellingService.Bestelling, BestellingCM>();

                conf.CreateMap<Klant, CommonModels.BffWebshop.KlantBeheer.Klant>();
                conf.CreateMap<Adres, CommonModels.BffWebshop.KlantBeheer.Adres>();

                conf.CreateMap<Bestelling, BevestigdeBestelling>();
                conf.CreateMap<BevestigdeBestelling, Bestelling>();
                conf.CreateMap<BevestigdeBestelRegel, BestelRegel>();
                conf.CreateMap<BestelRegel, BevestigdeBestelRegel>();
                conf.CreateMap<ContactInfo, BevestigdeBestellingContactInfo>();
                conf.CreateMap<Afleveradres, BevestigdeBestellingAfleveradres>();
            });

            _configured = true;
        }
    }
}
