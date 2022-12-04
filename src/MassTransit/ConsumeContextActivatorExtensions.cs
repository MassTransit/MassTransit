namespace MassTransit
{
    using System;
    using Microsoft.Extensions.DependencyInjection;


    public static class ConsumeContextActivatorExtensions
    {
        /// <summary>
        /// If the <see cref="ConsumeContext" /> has an <see cref="IServiceProvider" /> or <see cref="IServiceScope" /> payload,
        /// use that payload to get the service or create an instance of the specified type.
        /// </summary>
        /// <param name="context"></param>
        /// <typeparam name="T">The service type</typeparam>
        /// <returns></returns>
        /// <exception cref="PayloadNotFoundException"></exception>
        public static T GetServiceOrCreateInstance<T>(this ConsumeContext context)
            where T : class
        {
            if (context.TryGetPayload(out IServiceScope serviceScope))
                return ActivatorUtilities.GetServiceOrCreateInstance<T>(serviceScope.ServiceProvider);

            if (context.TryGetPayload(out IServiceProvider serviceProvider))
                return ActivatorUtilities.GetServiceOrCreateInstance<T>(serviceProvider);

            return ActivatorUtilities.CreateInstance<T>(Provider.Empty);
        }

        /// <summary>
        /// If the <see cref="ConsumeContext" /> has an <see cref="IServiceProvider" /> or <see cref="IServiceScope" /> payload,
        /// use that payload to create an instance of the specified type.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="T">The service type</typeparam>
        /// <returns></returns>
        /// <exception cref="PayloadNotFoundException"></exception>
        public static T CreateInstance<T>(this ConsumeContext context, params object[] arguments)
            where T : class
        {
            if (context.TryGetPayload(out IServiceScope serviceScope))
                return ActivatorUtilities.CreateInstance<T>(serviceScope.ServiceProvider, arguments);

            if (context.TryGetPayload(out IServiceProvider serviceProvider))
                return ActivatorUtilities.CreateInstance<T>(serviceProvider, arguments);

            return ActivatorUtilities.CreateInstance<T>(Provider.Empty);
        }


        static class Provider
        {
            internal static readonly IServiceProvider Empty = new EmptyServiceProvider();


            class EmptyServiceProvider :
                IServiceProvider
            {
                public object GetService(Type serviceType)
                {
                    return null;
                }
            }
        }
    }
}
