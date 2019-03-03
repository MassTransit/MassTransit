namespace MassTransit
{
    using System.Threading.Tasks;
    using Initializers;


    public static class MessageInitializerExtensions
    {
        /// <summary>
        /// Initialize a message using the specified input values (via an anonymous object, or an actual object)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="values"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Task<T> Init<T>(this ConsumeContext context, object values)
            where T : class
        {
            return MessageInitializerCache<T>.InitializeMessage(context, values);
        }
    }
}
