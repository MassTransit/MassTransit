// Copyright 2007-2011 The Apache Software Foundation.
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
namespace MassTransit.Transports.RabbitMq
{
    using System;
    using Configuration;
    using Exceptions;
    using Internal;
    using Magnum;
    using RabbitMQ.Client;

    public class RabbitMqEndpointConfigurator :
        EndpointConfiguratorBase
    {
        static readonly RabbitMqEndpointConfiguratorDefaults _defaults = new RabbitMqEndpointConfiguratorDefaults();

        public IEndpoint New(Action<IEndpointConfigurator> action)
        {
            action(this);

            Guard.AgainstNull(Uri, "No Uri was specified for the endpoint");
            if(MessageSerializer == null)
                Guard.AgainstNull(SerializerType, "No serializer type was specified for the endpoint");

            var settings = new CreateEndpointSettings(Uri)
                {
                    Serializer = GetSerializer(),
                    CreateIfMissing = _defaults.CreateMissingQueues,
                    PurgeExistingMessages = _defaults.PurgeOnStartup,
                    Transactional = _defaults.CreateTransactionalQueue,
                };

            try
            {
                Guard.AgainstNull(settings.Address, "An address for the endpoint must be specified");
                Guard.AgainstNull(settings.ErrorAddress, "An error address for the endpoint must be specified");
                Guard.AgainstNull(settings.Serializer, "A message serializer for the endpoint must be specified");

                EnsureProtocolIsCorrect(settings.Address.Uri);
                EnsureProtocolIsCorrect(settings.ErrorAddress.Uri);

                var tf = new RabbitMqTransportFactory();
                var transport = tf.New(settings.ToTransportSettings());

                PurgeExistingMessagesIfRequested(settings);

                var errorSettings = new CreateEndpointSettings(settings.ErrorAddress, settings);
                var errorTransport = tf.New(errorSettings.ToTransportSettings());

                var endpoint = new Endpoint(settings.Address, settings.Serializer, transport, errorTransport);

                return endpoint;
            }
            catch (Exception ex)
            {
                throw new EndpointException(settings.Address.Uri, "Failed to create NMS endpoint", ex);
            }
            
        }

        void PurgeExistingMessagesIfRequested(CreateEndpointSettings settings)
        {
            if(settings.Address.IsLocal && settings.PurgeExistingMessages)
            {
                //do it
            }
        }


        static void EnsureProtocolIsCorrect(Uri address)
        {
            if (address.Scheme != "rabbitmq")
                throw new EndpointException(address, "Address must start with 'rabbitmq' not '{0}'".FormatWith(address.Scheme));
        }

    }

    public class RabbitMqTransportFactory :
        ITransportFactory
    {
        static readonly ConnectionFactory _factory = new ConnectionFactory();
        static readonly IProtocol _protocol = Protocols.AMQP_0_8;


        public string Scheme
        {
            get { return "rabbitmq"; }
        }

        public ITransport New(CreateTransportSettings settings)
        {
            var transport = new RabbitMqTransport(settings.Address, GetConnection(settings.Address.Uri));
            return transport;
        }

        public static void Connect()
        {
            _factory.UserName = "guest";
            _factory.Password = "guest";
            _factory.VirtualHost = @"/";
            _factory.HostName = "";
        }

        static IConnection GetConnection(Uri address)
        {
            var rabbitMqAddress = new UriBuilder("amqp-{0}-{1}".FormatWith(_protocol.MajorVersion, _protocol.MinorVersion), address.Host, _protocol.DefaultPort).Uri;
            return _factory.CreateConnection();
        }
    }

    public class RabbitMqEndpointConfiguratorDefaults
    {
        public bool CreateMissingQueues { get; set; }

        public bool PurgeOnStartup { get; set; }

        public bool CreateTransactionalQueue { get; set; }
    }
}