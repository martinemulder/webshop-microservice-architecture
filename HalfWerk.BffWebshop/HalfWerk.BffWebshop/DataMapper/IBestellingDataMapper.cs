using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HalfWerk.CommonModels.BffWebshop.BestellingService;

namespace HalfWerk.BffWebshop.DataMapper
{
    public interface IBestellingDataMapper : IDataMapper<BevestigdeBestelling, long>
    {
        BevestigdeBestelling GetByFactuurnummer(long factuurnummer);
    }
}
