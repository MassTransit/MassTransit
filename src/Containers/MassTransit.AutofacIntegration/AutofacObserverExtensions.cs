namespace MassTransit
{
    using Autofac;
    using AutofacIntegration;
    using GreenPipes;
    using Pipeline;


    public static class AutofacObserverExtensions
    {
        /// <summary>
        /// Registers an <see cref="IConsumeObserver"/> which resolves the actual observer from the container lifetime scope
        /// </summary>
        /// <param name="connector"></param>
        /// <returns></returns>
        public static ConnectHandle ConnectAutofacConsumeObserver(this IConsumeObserverConnector connector)
        {
            var observer = new AutofacConsumeObserver();

            return connector.ConnectConsumeObserver(observer);
        }

        /// <summary>
        /// Registers an <see cref="IConsumeObserver"/> which resolves the actual observer from the container lifetime scope
        /// </summary>
        /// <param name="connector"></param>
        /// <param name="lifetimeScope">The default lifetime scope</param>
        /// <returns></returns>
        public static ConnectHandle ConnectAutofacConsumeObserver(this IConsumeObserverConnector connector, ILifetimeScope lifetimeScope)
        {
            var observer = new AutofacConsumeObserver(lifetimeScope);

            return connector.ConnectConsumeObserver(observer);
        }

        /// <summary>
        /// Registers an <see cref="IConsumeMessageObserver{T}"/> which resolves the actual observer from the container lifetime scope
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="connector"></param>
        /// <returns></returns>
        public static ConnectHandle ConnectAutofacConsumeMessageObserver<T>(this IConsumeMessageObserverConnector connector)
            where T : class
        {
            var observer = new AutofacConsumeMessageObserver<T>();

            return connector.ConnectConsumeMessageObserver(observer);
        }

        /// <summary>
        /// Registers an <see cref="IConsumeMessageObserver{T}"/> which resolves the actual observer from the container lifetime scope
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="connector"></param>
        /// <param name="lifetimeScope">The default lifetime scope</param>
        /// <returns></returns>
        public static ConnectHandle ConnectAutofacConsumeMessageObserver<T>(this IConsumeMessageObserverConnector connector, ILifetimeScope lifetimeScope)
            where T : class
        {
            var observer = new AutofacConsumeMessageObserver<T>(lifetimeScope);

            return connector.ConnectConsumeMessageObserver(observer);
        }
    }
}
