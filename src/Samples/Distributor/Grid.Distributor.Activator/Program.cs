namespace Grid.Distributor.Activator
{
    using System;
    using System.Configuration;
    using MassTransit.Log4NetIntegration.Logging;

    class Program
    {
        static void Main()
        {
            Log4NetLogger.Use("activator.log4net.config");

            try
            {
                var service = new ActivorServiceProvider
                    {
                        ServiceName = ConfigurationManager.AppSettings["ServiceName"],
                        DisplayName = ConfigurationManager.AppSettings["DisplayName"],
                        Description = ConfigurationManager.AppSettings["Description"],
                        SourceQueue = ConfigurationManager.AppSettings["SourceQueue"],
                    };

                service.ConfigureService<CollectCompletedWork>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}