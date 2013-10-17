namespace Grid.Distributor.Activator
{
    using MassTransit.Log4NetIntegration.Logging;
    using Topshelf;
    using Topshelf.Logging;


    class Program
    {
        static int Main()
        {
            Log4NetLogger.Use("activator.log4net.config");
            Log4NetLogWriterFactory.Use();

            return (int)HostFactory.Run(c =>
                {
                    c.SetServiceName("Grid.Distributor.Activator");

                    c.RunAsLocalSystem();

                    c.Service<ActivatorService>();
                });
        }
    }
}