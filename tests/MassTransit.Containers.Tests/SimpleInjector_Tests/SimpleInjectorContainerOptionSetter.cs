namespace MassTransit.Containers.Tests.SimpleInjector_Tests
{
    using SimpleInjector;
    using SimpleInjector.Lifestyles;


    static class SimpleInjectorContainerOptionSetter
    {
        public static void SetMassTransitContainerOptions(this Container container)
        {
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            container.Options.EnableAutoVerification = false;
            container.Options.ResolveUnregisteredConcreteTypes = false;
        }
    }
}
