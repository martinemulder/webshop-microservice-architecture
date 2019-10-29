using HalfWerk.CommonModels.DsBestelService.Models;

namespace HalfWerk.DsBestelService.DAL.DataMappers
{
    public interface IBestellingDataMapper : IDataMapper<Bestelling, long>
    {
        Bestelling GetByFactuurnummer(long factuurnummer);
    }
}