namespace RapidTransit
{
    using Autofac;
    using Autofac.Builder;
    using MassTransit;
    using MassTransit.AutofacIntegration;


    public static class ContainerBuilderExtensions
    {
        const string MessageScopeTag = "message";

        public static void RegisterAutofacConsumerFactory(this ContainerBuilder builder)
        {
            // the ConsumerFactory uses message scope so that related components are all created within the same
            // container lifetime
            builder.RegisterGeneric(typeof(AutofacConsumerFactory<>))
                   .WithParameter(new NamedParameter("name", MessageScopeTag))
                   .As(typeof(IConsumerFactory<>));
        }

        public static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> InstancePerMessageScope
            <TLimit, TActivatorData, TRegistrationStyle>(
            this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> builder)
        {
            return builder.InstancePerMatchingLifetimeScope(MessageScopeTag);
        }
    }
}