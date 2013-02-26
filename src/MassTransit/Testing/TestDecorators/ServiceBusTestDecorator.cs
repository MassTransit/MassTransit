// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Testing.TestDecorators
{
    using System;
    using Context;
    using Diagnostics.Introspection;
    using Magnum.Reflection;
    using Pipeline;
    using Scenarios;

    public class ServiceBusTestDecorator :
        IServiceBus
    {
        readonly IServiceBus _bus;
        readonly PublishedMessageListImpl _published;
        readonly EndpointTestScenarioImpl _scenario;

        public ServiceBusTestDecorator(IServiceBus bus, EndpointTestScenarioImpl scenario)
        {
            _bus = bus;
            _scenario = scenario;

            _published = new PublishedMessageListImpl();
        }

        public void Inspect(DiagnosticsProbe probe)
        {
            _bus.Inspect(probe);
        }

        public void Dispose()
        {
            _bus.Dispose();
        }

        public IEndpoint Endpoint
        {
            get { return _bus.Endpoint; }
        }

        public IInboundMessagePipeline InboundPipeline
        {
            get { return _bus.InboundPipeline; }
        }

        public IOutboundMessagePipeline OutboundPipeline
        {
            get { return _bus.OutboundPipeline; }
        }

        public IServiceBus ControlBus
        {
            get { return _bus.ControlBus; }
        }

        public IEndpointCache EndpointCache
        {
            get { return _bus.EndpointCache; }
        }

        public TimeSpan ShutdownTimeout { get; set; }

        public void Publish<T>(T message)
            where T : class
        {
            Publish(message, NoContext);
        }

        public void Publish<T>(T message, Action<IPublishContext<T>> contextCallback)
            where T : class
        {
            PublishedMessageImpl<T> published = null;
            try
            {
                _bus.Publish(message, context =>
                    {
                        published = new PublishedMessageImpl<T>(context);

                        contextCallback(context);
                    });
            }
            catch (Exception ex)
            {
                if (published != null)
                    published.SetException(ex);
                throw;
            }
            finally
            {
                if (published != null)
                {
                    _published.Add(published);
                    _scenario.AddPublished(published);
                }
            }
        }

        public void Publish(object message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            BusObjectPublisherCache.Instance[message.GetType()].Publish(this, message);
        }

        public void Publish(object message, Type messageType)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (messageType == null)
                throw new ArgumentNullException("messageType");

            BusObjectPublisherCache.Instance[messageType].Publish(this, message);
        }

        public void Publish(object message, Action<IPublishContext> contextCallback)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (contextCallback == null)
                throw new ArgumentNullException("contextCallback");

            BusObjectPublisherCache.Instance[message.GetType()].Publish(this, message, contextCallback);
        }

        public void Publish(object message, Type messageType, Action<IPublishContext> contextCallback)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (messageType == null)
                throw new ArgumentNullException("messageType");
            if (contextCallback == null)
                throw new ArgumentNullException("contextCallback");

            BusObjectPublisherCache.Instance[messageType].Publish(this, message);
        }

        public void Publish<T>(object values)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException("values");

            var message = InterfaceImplementationExtensions.InitializeProxy<T>(values);

            Publish(message, x => { });
        }

        public void Publish<T>(object values, Action<IPublishContext<T>> contextCallback) 
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException("values");

            var message = InterfaceImplementationExtensions.InitializeProxy<T>(values);

            Publish(message, contextCallback);
        }

        public IEndpoint GetEndpoint(Uri address)
        {
            return _bus.GetEndpoint(address);
        }

        public UnsubscribeAction Configure(Func<IInboundPipelineConfigurator, UnsubscribeAction> configure)
        {
            return _bus.Configure(configure);
        }

        void NoContext<T>(IPublishContext<T> context)
            where T : class
        {
        }
        
        public IBusService GetService(Type type)
        {
            return _bus.GetService(type);
        }

        public bool TryGetService(Type type, out IBusService result)
        {
            return _bus.TryGetService(type, out result);
        }
    }
}