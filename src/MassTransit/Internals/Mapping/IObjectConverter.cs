namespace MassTransit.Internals.Mapping
{
    public interface IObjectConverter
    {
        object GetObject(IObjectValueProvider valueProvider);
    }
}