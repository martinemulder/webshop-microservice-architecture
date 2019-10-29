using HalfWerk.CommonModels.PcsBetalingService.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HalfWerk.PcsBetaalService.Services
{
    public interface IBetalingVerwerkenService
    {
        void HandleBestellingVerwerken(long factuurnummer);
        void HandleBetalingVerwerken(Betaling betaling);
    }
}
