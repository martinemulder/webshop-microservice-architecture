using AutoMapper;
using HalfWerk.CommonModels.DsBestelService.Models;

namespace HalfWerk.DsBestelService
{
    public static class AutoMapperConfiguration
    {
        private static bool _configured;

        public static void Configure()
        {
            if (_configured)
            {
                return;
            }

            Mapper.Initialize(config =>
            {
                config.CreateMap<ContactInfoCM, ContactInfo>();
                config.CreateMap<AfleveradresCM, Afleveradres>();
                config.CreateMap<Artikel, BestelRegel>();
            });

            _configured = true;
        }
    }
}