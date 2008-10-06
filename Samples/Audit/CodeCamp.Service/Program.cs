namespace CodeCamp.Service
{
    using MassTransit.Host;

    internal class Program
    {
        private static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            var cfg = new AuditServiceConfiguration("audit-castle.config");
            Runner.Run(cfg, args);
        }
    }
}