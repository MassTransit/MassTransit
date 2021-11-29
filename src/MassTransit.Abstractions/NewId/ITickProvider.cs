namespace MassTransit
{
    public interface ITickProvider
    {
        long Ticks { get; }
    }
}
