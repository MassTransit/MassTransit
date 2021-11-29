namespace MassTransit.Transports
{
    using System.Threading.Tasks;


    public interface IPublishPipe :
        IProbeSite
    {
        Task Send<T>(PublishContext<T> context)
            where T : class;
    }
}
