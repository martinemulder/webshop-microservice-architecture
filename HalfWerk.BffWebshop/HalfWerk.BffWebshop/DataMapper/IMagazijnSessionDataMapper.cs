using HalfWerk.BffWebshop.Entities;

namespace HalfWerk.BffWebshop.DataMapper
{
    public interface IMagazijnSessionDataMapper : IDataMapper<MagazijnSessionEntity, long>
    {
        MagazijnSessionEntity GetByFactuurnummer(long factuurnummer);
    }
}