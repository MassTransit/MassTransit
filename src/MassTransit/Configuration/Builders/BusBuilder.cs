// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using BusConfigurators;
    using GreenPipes;
    using Pipeline;
    using Transports;
    using Util;


    public abstract class BusBuilder :
        IBusBuilder
    {
        readonly BusObservable _busObservable;
        readonly Lazy<IConsumePipe> _consumePipe;
        readonly IConsumePipeFactory _consumePipeFactory;
        readonly IBusHostCollection _hosts;
        readonly Lazy<Uri> _inputAddress;
        readonly IPublishPipeFactory _publishPipeFactory;
        readonly ISendPipeFactory _sendPipeFactory;
        readonly Lazy<ISendTransportProvider> _sendTransportProvider;
        readonly SerializerBuilder _serializerBuilder;

        protected BusBuilder(IConsumePipeFactory consumePipeFactory, ISendPipeFactory sendPipeFactory,
            IPublishPipeFactory publishPipeFactory, IBusHostCollection hosts)
        {
            _consumePipeFactory = consumePipeFactory;
            _sendPipeFactory = sendPipeFactory;
            _publishPipeFactory = publishPipeFactory;
            _hosts = hosts;

            _serializerBuilder = new SerializerBuilder();

            _busObservable = new BusObservable();
            _sendTransportProvider = new Lazy<ISendTransportProvider>(CreateSendTransportProvider);

            _inputAddress = new Lazy<Uri>(GetInputAddress);
            _consumePipe = new Lazy<IConsumePipe>(GetConsumePipe);
        }

        protected BusObservable BusObservable => _busObservable;
        protected Uri InputAddress => _inputAddress.Value;
        protected IConsumePipe ConsumePipe => _consumePipe.Value;

        public abstract ISendEndpointProvider SendEndpointProvider { get; }
        public abstract IPublishEndpointProvider PublishEndpointProvider { get; }

        public ISendTransportProvider SendTransportProvider => _sendTransportProvider.Value;

        public void AddMessageDeserializer(ContentType contentType, DeserializerFactory deserializerFactory)
        {
            _serializerBuilder.AddDeserializer(contentType, deserializerFactory);
        }

        public void SetMessageSerializer(SerializerFactory serializerFactory)
        {
            _serializerBuilder.SetSerializer(serializerFactory);
        }

        public ISendPipe CreateSendPipe(params ISendPipeSpecification[] specifications)
        {
            return _sendPipeFactory.CreateSendPipe(specifications);
        }

        public IConsumePipe CreateConsumePipe(params IConsumePipeSpecification[] specifications)
        {
            return _consumePipeFactory.CreateConsumePipe(specifications);
        }

        public ConnectHandle ConnectBusObserver(IBusObserver observer)
        {
            return _busObservable.Connect(observer);
        }

        public SerializerBuilder CreateSerializerBuilder()
        {
            return new SerializerBuilder(_serializerBuilder);
        }

        public IPublishPipe CreatePublishPipe(params IPublishPipeSpecification[] specifications)
        {
            return _publishPipeFactory.CreatePublishPipe(specifications);
        }

        protected abstract Uri GetInputAddress();
        protected abstract IConsumePipe GetConsumePipe();

        public IBusControl Build()
        {
            try
            {
                PreBuild();

                var bus = new MassTransitBus(InputAddress, ConsumePipe, SendEndpointProvider, PublishEndpointProvider, _hosts, BusObservable);

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

        protected abstract ISendTransportProvider CreateSendTransportProvider();
    }
}