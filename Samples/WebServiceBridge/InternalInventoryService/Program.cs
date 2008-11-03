namespace InternalInventoryService
{
    using log4net;
    using MassTransit.Host;
    using MassTransit.Host.Configurations;
    using MassTransit.WindsorIntegration;
    using Microsoft.Practices.ServiceLocation;

    internal static class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

        private static void Main(string[] args)
        {
            _log.Info("InternalInventoryService Loading...");

            var container = new DefaultMassTransitContainer("InternalInventoryService.Castle.xml");
            container.AddComponent<InventoryLevelService>();

            var wob = new WindsorObjectBuilder(container.Kernel);
            ServiceLocator.SetLocatorProvider(()=>wob);

            var credentials = Credentials.LocalSystem;
            var settings = WinServiceSettings.Custom(
                "InternalInventoryService",
                "Internal Inventory Service",
                "Handles inventory for internal systems",
                KnownServiceNames.Msmq);
            var lifecycle = new InternalInventoryServiceLifeCycle(ServiceLocator.Current);

            Runner.Run(credentials, settings, lifecycle, args);
        }
    }
}