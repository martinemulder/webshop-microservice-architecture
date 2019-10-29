namespace HalfWerk.BffWebshop.Entities
{
    public interface IEntity<out T>
    {
        T GetKeyValue();
    }
}
