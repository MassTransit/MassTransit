namespace RapidTransit
{
    using Autofac.Builder;


    public static class ServiceBootstrapperExtensions
    {
        public static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> InstancePerServiceScope
            <TLimit, TActivatorData, TRegistrationStyle>(
            this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> builder,
            IServiceBootstrapper bootstrapper)
        {
            return builder.InstancePerMatchingLifetimeScope(bootstrapper.LifetimeScopeTag);
        }
    }
}