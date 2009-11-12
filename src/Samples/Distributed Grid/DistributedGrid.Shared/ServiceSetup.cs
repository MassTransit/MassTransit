using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MassTransit.StructureMapIntegration;
using MassTransit.Transports.Msmq;
using StructureMap;
using StructureMap.Configuration.DSL;
using Microsoft.Practices.ServiceLocation;
using Topshelf;
using Topshelf.Configuration;

namespace DistributedGrid.Shared
{
    public interface IServiceInterface
    {
        void Start();
        void Stop();
    }

    public abstract class ServiceSetup
    {
        abstract public string ServiceName { get; set; }
        abstract public string DisplayName { get; set; }
        abstract public string Description { get; set; }
        abstract public string SourceQueue { get; set; }
        abstract public string SubscriptionQueue { get; set; }
        abstract public Action<ConfigurationExpression> ContainerSetup { get; set; }

        public void ConfigureService<T>(string[] args) where T : class, IServiceInterface
        {
            IRunConfiguration cfg = RunnerConfigurator.New(c =>
            {
                c.SetServiceName(ServiceName);
                c.SetDisplayName(DisplayName);
                c.SetDescription(Description);

                c.RunAsLocalSystem();
                c.DependencyOnMsmq();

                c.ConfigureService<T>(typeof(T).Name, s =>
                {
                    MsmqEndpointConfigurator.Defaults(def => def.CreateMissingQueues = true);
                    s.CreateServiceLocator(() =>
                    {
                        Container container = new Container(x =>
                        {
                            x.AddRegistry(new IocRegistry(SourceQueue, SubscriptionQueue));

                            ContainerSetup(x);
                        });

                        IServiceLocator objectBuilder = new StructureMapObjectBuilder(container);
                        ServiceLocator.SetLocatorProvider(() => objectBuilder);
                        return objectBuilder;
                    });

                    s.WhenStarted(start => start.Start());
                    s.WhenStopped(stop => stop.Stop());
                });
            });

            ObjectFactory.AssertConfigurationIsValid();

            Runner.Host(cfg, args);
        }

    }
}
