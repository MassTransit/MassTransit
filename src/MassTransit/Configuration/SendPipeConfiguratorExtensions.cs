namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using Configuration;


    public static class DelegatePipeConfiguratorExtensions
    {
        /// <summary>
        /// Adds a callback filter to the send pipeline
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="callback">The callback to invoke</param>
        public static void UseSendExecute(this ISendPipeConfigurator configurator, Action<SendContext> callback)
        {
            var specification = new DelegatePipeSpecification<SendContext>(callback);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Adds a callback filter to the send pipeline
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="callback">The callback to invoke</param>
        public static void UseSendExecuteAsync(this ISendPipeConfigurator configurator, Func<SendContext, Task> callback)
        {
            var specification = new AsyncDelegatePipeSpecification<SendContext>(callback);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Adds a callback filter to the send pipeline
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="callback">The callback to invoke</param>
        public static void UseSendExecute<T>(this ISendPipeConfigurator configurator, Action<SendContext<T>> callback)
            where T : class
        {
            var specification = new DelegatePipeSpecification<SendContext<T>>(callback);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Adds a callback filter to the send pipeline
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="callback">The callback to invoke</param>
        public static void UseSendExecuteAsync<T>(this ISendPipeConfigurator configurator, Func<SendContext<T>, Task> callback)
            where T : class
        {
            var specification = new AsyncDelegatePipeSpecification<SendContext<T>>(callback);

            configurator.AddPipeSpecification(specification);
        }
    }
}
