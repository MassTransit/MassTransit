using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace DistributedGrid.Activator
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new ActivorServiceProvider()
            {
                ServiceName = ConfigurationManager.AppSettings["ServiceName"],
                DisplayName = ConfigurationManager.AppSettings["DisplayName"],
                Description = ConfigurationManager.AppSettings["Description"],
                SourceQueue = ConfigurationManager.AppSettings["SourceQueue"],
                SubscriptionQueue = ConfigurationManager.AppSettings["SubscriptionQueue"],
                ContainerSetup = x => { }
            };

            service.ConfigureService<CollectCompletedWork>(args);
        }
    }
}
