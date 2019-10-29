using AutoMapper;
using HalfWerk.CommonModels.PcsBetalingService.Models;

namespace HalfWerk.PcsBetaalService
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
                config.CreateMap<BetalingCM, Betaling>();
            });

            _configured = true;
        }
    }
}
