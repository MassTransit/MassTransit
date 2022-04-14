namespace MassTransit.Middleware
{
    using System.Threading.Tasks;


    public interface IOutboxContextFactory<TContext> :
        IProbeSite
        where TContext : class
    {
        Task Send<T>(ConsumeContext<T> context, OutboxConsumeOptions options, IPipe<OutboxConsumeContext<T>> next)
            where T : class;
    }
}
