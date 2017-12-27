// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Builders
{
    using System;
    using System.Net.Mime;
    using EndpointSpecifications;
    using GreenPipes;
    using Pipeline;
    using Pipeline.Observables;
    using Transports;
    using Util;


    public abstract class BusBuilder :
        IBusBuilder
    {
        readonly BusObservable _busObservable;
        readonly IBusHostCollection _hosts;
        readonly Lazy<Uri> _inputAddress;
        readonly SerializerBuilder _serializerBuilder;
        readonly IEndpointConfiguration _configuration;
        readonly IConsumePipe _consumePipe;

        protected BusBuilder(IBusHostCollection hosts, IEndpointConfiguration configuration)
        {
            _hosts = hosts;
            _configuration = configuration;

            _serializerBuilder = new SerializerBuilder();

            _busObservable = new BusObservable();

            _inputAddress = new Lazy<Uri>(GetInputAddress);

            _consumePipe = _configuration.Consume.CreatePipe();
        }

        protected BusObservable BusObservable => _busObservable;
        protected Uri InputAddress => _inputAddress.Value;
        protected IConsumePipe ConsumePipe => _consumePipe;

        public abstract ISendEndpointProvider SendEndpointProvider { get; }
        public abstract IPublishEndpointProvider PublishEndpointProvider { get; }

        public void AddMessageDeserializer(ContentType contentType, DeserializerFactory deserializerFactory)
        {
            _serializerBuilder.AddDeserializer(contentType, deserializerFactory);
        }

        public void SetMessageSerializer(SerializerFactory serializerFactory)
        {
            _serializerBuilder.SetSerializer(serializerFactory);
        }

        public ISendPipe CreateSendPipe()
        {
            return _configuration.Send.CreatePipe();
        }

        public IConsumePipe CreateConsumePipe()
        {
            return _consumePipe;
        }

        public ConnectHandle ConnectBusObserver(IBusObserver observer)
        {
            return _busObservable.Connect(observer);
        }

        public SerializerBuilder CreateSerializerBuilder()
        {
            return new SerializerBuilder(_serializerBuilder);
        }

        public IPublishPipe CreatePublishPipe()
        {
            return _configuration.Publish.CreatePipe();
        }

        protected abstract Uri GetInputAddress();

        public IBusControl Build()
        {
            try
            {
                PreBuild();

                var hostTopology = _hosts.GetHost(InputAddress).Topology;

                var bus = new MassTransitBus(InputAddress, ConsumePipe, SendEndpointProvider, PublishEndpointProvider, _hosts, BusObservable, hostTopology);

                TaskUtil.Await(() => _busObservable.PostCreate(bus));

                return bus;
            }
            catch (Exception exception)
            {
                TaskUtil.Await(() => BusObservable.CreateFaulted(exception));

                throw;
            }
        }

        protected virtual void PreBuild()
        {
        }
    }
}