namespace MassTransit.Transports
{
    using System.IO;

    public interface ISendingContext
    {
        Stream Body { get; set; }
    }
}