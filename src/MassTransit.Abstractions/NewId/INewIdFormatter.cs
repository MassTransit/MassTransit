namespace MassTransit
{
    public interface INewIdFormatter
    {
        string Format(in byte[] bytes);
    }
}
