namespace InternalInventoryService
{
    using log4net;
    using MassTransit.Host;

    internal static class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

        private static void Main(string[] args)
        {
            _log.Info("InternalInventoryService Loading...");

            IInstallationConfiguration cfg = new InternalInventoryServiceEnvironment("InternalInventoryService.Castle.xml");

            Runner.Run(cfg, args);
        }
    }
}