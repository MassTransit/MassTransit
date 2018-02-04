namespace MassTransit.Initializers
{
    using System.Threading.Tasks;


    /// <summary>
    /// A message initializer that uses the input
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    /// <typeparam name="TInput">The input type</typeparam>
    public interface IPropertyInitializer<in TMessage, in TInput>
        where TMessage : class
        where TInput : class
    {
        Task Apply(InitializeContext<TMessage, TInput> context);
    }


    /// <summary>
    /// A message initializer that doesn't use the input
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    public interface IPropertyInitializer<in TMessage>
        where TMessage : class
    {
        /// <summary>
        /// Apply the initializer to the message
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task Apply(InitializeContext<TMessage> context);
    }
}
