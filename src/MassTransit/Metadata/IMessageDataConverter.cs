namespace MassTransit.Metadata
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;


    public interface IMessageDataConverter<T>
    {
        Task<T> Convert(Stream stream, CancellationToken cancellationToken);
    }
}
