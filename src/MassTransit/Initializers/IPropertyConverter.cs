namespace MassTransit.Initializers
{
    using System.Threading.Tasks;


    /// <summary>
    /// A message property converter, which is async, and has access to the context
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    public interface IPropertyConverter<TResult, in TProperty>
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<TResult> Convert<T>(InitializeContext<T> context, TProperty input)
            where T : class;
    }
}
