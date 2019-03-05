namespace MassTransit.Initializers
{
    using System.Threading.Tasks;


    /// <summary>
    /// Initialize a message header
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    public interface IHeaderInitializer<in TMessage, in TInput>
        where TMessage : class
        where TInput : class
    {
        Task Apply(InitializeContext<TMessage, TInput> context, SendContext sendContext);
    }


    /// <summary>
    /// Initialize a message header
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public interface IHeaderInitializer<in TMessage>
        where TMessage : class
    {
        Task Apply(InitializeContext<TMessage> context, SendContext sendContext);
    }
}
