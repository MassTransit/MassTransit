namespace Grid.Distributor.Worker
{
    using MassTransit.Log4NetIntegration.Logging;
    using Topshelf;
    using Topshelf.Logging;


    class Program
    {
        static int Main()
        {
            Log4NetLogger.Use("worker.log4net.config");
            Log4NetLogWriterFactory.Use();

            return (int)HostFactory.Run(c =>
                {
                    c.SetServiceName("Grid.Distributor.Worker");

                    c.RunAsLocalSystem();

                    c.Service<WorkerService>();
                });
        }
    }
}