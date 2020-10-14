namespace MassTransit.Containers.Tests
{
    using SimpleInjector;
    using SimpleInjector.Lifestyles;

    internal static class SimpleInjectorContainerOptionSetter
    {
        public static void SetRequiredOptions(this Container container)
        {
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            container.Options.EnableAutoVerification = false;
        }
    }
}
