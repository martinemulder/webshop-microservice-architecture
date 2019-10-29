using HalfWerk.CommonModels.PcsBetalingService.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HalfWerk.PcsBetaalService.DAL.DataMappers
{
    public interface IBestellingDataMapper : IDataMapper<Bestelling, long>
    {
        Bestelling GetByFactuurnummer(long factuurnummer);
    }
}
