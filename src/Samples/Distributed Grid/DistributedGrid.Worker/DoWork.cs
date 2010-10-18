// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace DistributedGrid.Worker
{
    using System;
    using System.Configuration;
    using MassTransit;
    using MassTransit.Configuration;
    using MassTransit.Services.Subscriptions.Configuration;
    using Shared;
    using Shared.Messages;
    using log4net;

    public class DoWork :
        IServiceInterface,
        IDisposable,
        Consumes<DoSimpleWorkItem>.All
    {
        private ILog _log = LogManager.GetLogger(typeof(DoWork));

        public DoWork(IObjectBuilder objectBuilder)
        {
            ObjectBuilder = objectBuilder;
        }

        public void Start()
        {
            ControlBus = ControlBusConfigurator.New(x =>
                {
                    x.SetObjectBuilder(ObjectBuilder);

                    x.ReceiveFrom(ConfigurationManager.AppSettings["SourceQueue"] + "_control");

                    x.PurgeBeforeStarting();
                });

            DataBus = ServiceBusConfigurator.New(x =>
                {
                    x.SetObjectBuilder(ObjectBuilder);
                    x.ConfigureService<SubscriptionClientConfigurator>(y =>
                        {
                            y.SetSubscriptionServiceEndpoint(ConfigurationManager.AppSettings["SubscriptionQueue"]);
                        });
                    x.ReceiveFrom(ConfigurationManager.AppSettings["SourceQueue"]);
                    x.UseControlBus(ControlBus);
                    x.SetConcurrentConsumerLimit(4);

                    x.ConfigureService<GridConfigurator>(grid =>
                        {
                            if (ConfigurationManager.AppSettings["IsProposer"].Equals("true"))
                                grid.SetProposer();
                            grid.For<DoSimpleWorkItem>().Use<DoWork>();
                        });
                });
        }

        public void Stop()
        {
            DataBus.Dispose();
        }

        public IServiceBus DataBus { get; private set; }
        public IControlBus ControlBus { get; private set; }
        public IObjectBuilder ObjectBuilder { get; private set; }

        public void Dispose()
        {
        }

        public void Consume(DoSimpleWorkItem message)
        {
            _log.InfoFormat("Responding to {0}", message.CorrelationId);
            CurrentMessage.Respond(new CompletedSimpleWorkItem(message.CorrelationId, message.CreatedAt));
        }
    }
}