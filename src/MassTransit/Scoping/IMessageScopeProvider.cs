namespace MassTransit.Scoping
{
    using System.Threading.Tasks;
    using GreenPipes;


    public interface IMessageScopeProvider :
        IProbeSite
    {
        ValueTask<IMessageScopeContext<T>> GetScope<T>(ConsumeContext<T> context)
            where T : class;
    }
}
