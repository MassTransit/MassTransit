namespace Grid.Distributor.Worker
{
    using System;
    using System.Configuration;
    using MassTransit.Log4NetIntegration.Logging;

    class Program
    {
        static void Main()
        {
            Log4NetLogger.Use("worker.log4net.config");

            try
            {
                var service = new WorkerServiceProvider
                    {
                        ServiceName = ConfigurationManager.AppSettings["ServiceName"],
                        DisplayName = ConfigurationManager.AppSettings["DisplayName"],
                        Description = ConfigurationManager.AppSettings["Description"],
                        SourceQueue = ConfigurationManager.AppSettings["SourceQueue"],
                    };

                service.ConfigureService<DoWork>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}