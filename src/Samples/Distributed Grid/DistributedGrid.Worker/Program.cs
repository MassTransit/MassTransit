using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using log4net.Config;
using MassTransit;
using MassTransit.Grid;
using MassTransit.Grid.Paxos;
using MassTransit.Grid.Sagas;
using MassTransit.Saga;
using StructureMap;
using StructureMap.Attributes;

namespace DistributedGrid.Worker
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo(@"log4net.config"));

            var service = new WorkerServiceProvider()
            {
                ServiceName = ConfigurationManager.AppSettings["ServiceName"],
                DisplayName = ConfigurationManager.AppSettings["DisplayName"],
                Description = ConfigurationManager.AppSettings["Description"],
                SourceQueue = ConfigurationManager.AppSettings["SourceQueue"],
                SubscriptionQueue = ConfigurationManager.AppSettings["SubscriptionQueue"],
                ContainerSetup = x => 
                {
                    AddSagaRepositoryForType<Node>(x);
                    AddSagaRepositoryForType<ServiceType>(x);
                    AddSagaRepositoryForType<ServiceMessage>(x);
                    AddSagaRepositoryForType<ServiceNode>(x);
                    AddSagaRepositoryForType<Learner<AvailableGridServiceNode>>(x);
                    AddSagaRepositoryForType<Acceptor<AvailableGridServiceNode>>(x);
                    x.ForRequestedType<String>()
                        .AlwaysUnique()
                        .MissingNamedInstanceIs
                        .ConstructedBy(ic => ConfigurationManager.AppSettings[ic.RequestedName]);
                }
            };

            service.ConfigureService<DoWork>(args);
        }

        protected static void AddSagaRepositoryForType<T>(ConfigurationExpression x) where T : class, ISaga
        {
            x.ForRequestedType<ISagaRepository<T>>()
                .CacheBy(InstanceScope.Singleton)
                .TheDefaultIsConcreteType<InMemorySagaRepository<T>>();
        }
    }
}
