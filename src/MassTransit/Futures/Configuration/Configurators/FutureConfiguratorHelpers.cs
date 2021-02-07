namespace MassTransit.Futures.Configurators
{
    using System;


    static class FutureConfiguratorHelpers
    {
        static readonly object _defaultValues = new Default();

        internal static object DefaultProvider<T>(FutureConsumeContext<T> context)
            where T : class
        {
            return _defaultValues;
        }

        internal static object DefaultProvider(FutureConsumeContext context)
        {
            return _defaultValues;
        }

        internal static Uri PublishAddressProvider<T>(FutureConsumeContext<T> context)
            where T : class
        {
            return default;
        }


        class Default
        {
        }
    }
}
