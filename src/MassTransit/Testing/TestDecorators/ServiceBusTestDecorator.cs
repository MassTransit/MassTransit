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
    using System.Threading;
    using System.Threading.Tasks;
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
        Uri _address;
        IConsumePipe _consumePipe;

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

        public void Publish<T>(T message, Action<PublishContext<T>> contextCallback)
            where T : class
        {
            PublishedMessageImpl<T> published = null;
            try
            {
//                _bus.Publish(message, context =>
//                    {
//                        published = new PublishedMessageImpl<T>(context);
//
//                        contextCallback(context);
//                    });
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

        public IEndpoint GetEndpoint(Uri address)
        {
            return _bus.GetEndpoint(address);
        }


        void NoContext<T>(PublishContext<T> context)
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

        public Uri Address
        {
            get { return _address; }
        }

        public IConsumePipe ConsumePipe
        {
            get { return _consumePipe; }
        }

        Task<ISendEndpoint> ISendEndpointProvider.GetSendEndpoint(Uri address)
        {
            throw new NotImplementedException();
        }

        public Task Publish<T>(T message, CancellationToken cancellationToken = new CancellationToken()) where T : class
        {
            throw new NotImplementedException();
        }

        public Task Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken = new CancellationToken()) where T : class
        {
            throw new NotImplementedException();
        }

        public Task Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = new CancellationToken()) where T : class
        {
            throw new NotImplementedException();
        }

        public Task Publish(object message, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task Publish(object message, Type messageType, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task Publish<T>(object values, CancellationToken cancellationToken = new CancellationToken()) where T : class
        {
            throw new NotImplementedException();
        }

        public Task Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken = new CancellationToken()) where T : class
        {
            throw new NotImplementedException();
        }

        public Task Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = new CancellationToken()) where T : class
        {
            throw new NotImplementedException();
        }
    }
}