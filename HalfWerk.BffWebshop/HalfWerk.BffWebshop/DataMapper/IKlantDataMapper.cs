using HalfWerk.CommonModels.BffWebshop.KlantBeheer;

namespace HalfWerk.BffWebshop.DataMapper
{
    public interface IKlantDataMapper : IDataMapper<Klant, long>
    {
        Klant GetByEmail(string email);
    }
}