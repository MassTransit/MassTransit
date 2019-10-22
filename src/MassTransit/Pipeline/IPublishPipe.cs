namespace MassTransit.Pipeline
{
    using System.Threading.Tasks;
    using GreenPipes;


    public interface IPublishPipe :
        IProbeSite
    {
        Task Send<T>(PublishContext<T> context)
            where T : class;
    }
}
