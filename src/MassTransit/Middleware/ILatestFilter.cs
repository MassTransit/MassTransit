namespace MassTransit.Middleware
{
    using System.Threading.Tasks;


    /// <summary>
    /// Maintains the latest context to be passed through the filter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILatestFilter<T>
        where T : class, PipeContext
    {
        /// <summary>
        /// The most recently completed context to pass through the filter
        /// </summary>
        Task<T> Latest { get; }
    }
}
