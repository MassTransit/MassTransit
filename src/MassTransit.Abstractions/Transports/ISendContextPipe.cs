namespace MassTransit.Transports
{
    using System.Threading.Tasks;


    public interface ISendContextPipe
    {
        Task Send<T>(SendContext<T> context)
            where T : class;
    }
}
