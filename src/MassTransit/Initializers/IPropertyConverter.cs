namespace MassTransit.Initializers
{
    using System.Threading.Tasks;


    /// <summary>
    /// A message property converter, which is async, and has access to the context
    /// </summary>
    /// <typeparam name="TProperty"></typeparam>
    /// <typeparam name="TInputProperty"></typeparam>
    public interface IPropertyConverter<TProperty, in TInputProperty>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<TProperty> Convert<TMessage>(InitializeContext<TMessage> context, TInputProperty input)
            where TMessage : class;
    }
}
