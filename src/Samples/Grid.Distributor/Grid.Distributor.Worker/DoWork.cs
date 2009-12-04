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
using System.Configuration;
using MassTransit.Configuration;
using MassTransit.Distributor;
using MassTransit.Distributor.Configuration;
using MassTransit.Serialization;
using MassTransit.Services.Subscriptions.Configuration;
using MassTransit.Transports.Msmq;
using StructureMap.Pipeline;

namespace Grid.Distributor.Worker
{
    using System;
    using Shared;
    using Shared.Messages;
    using log4net;
    using MassTransit;

    public class DoWork :
        IServiceInterface,
        IDisposable,
        Consumes<DoSimpleWorkItem>.All
    {
        private ILog _log = LogManager.GetLogger(typeof(DoWork));
        private UnsubscribeAction _unsubscribeAction;

        public IObjectBuilder ObjectBuilder { get; set; }
        public IControlBus ControlBus { get; set; }
        public IServiceBus DataBus { get; set; }

        public DoWork(IObjectBuilder objectBuilder)
        {
            ObjectBuilder = objectBuilder;

            var endpointFactory = EndpointFactoryConfigurator.New(x =>
            {
                x.RegisterTransport<MsmqEndpoint>();
                x.SetObjectBuilder(objectBuilder);
                x.SetDefaultSerializer<XmlMessageSerializer>();
            });

            ControlBus = ControlBusConfigurator.New(x =>
            {
                x.SetObjectBuilder(ObjectBuilder);

                x.ReceiveFrom(new Uri(ConfigurationManager.AppSettings["SourceQueue"]).AppendToPath("_control"));

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
                x.ImplementDistributorWorker<DoSimpleWorkItem>(ConsumeMessage);
            });
        }

        public void Start()
        {
            _unsubscribeAction = DataBus.Subscribe(this);
        }

        public void Stop()
        {
            if (_unsubscribeAction != null)
                _unsubscribeAction();
        }

        public void Dispose()
        {
        }

        public void Consume(DoSimpleWorkItem message)
        {
            _log.InfoFormat("Responding to {0}", message.CorrelationId);
            CurrentMessage.Respond(new CompletedSimpleWorkItem(message.CorrelationId, message.CreatedAt));
        }

        public Action<DoSimpleWorkItem> ConsumeMessage(DoSimpleWorkItem message)
        {
            return m => { Consume(m); };
        }
    }
}