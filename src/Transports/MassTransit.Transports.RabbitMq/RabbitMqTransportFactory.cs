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
    using Exceptions;
    using RabbitMQ.Client;

    public class RabbitMqTransportFactory :
        ITransportFactory
    {
        static readonly ConnectionFactory _factory = new ConnectionFactory();
        static readonly IProtocol _protocol = Protocols.AMQP_0_8;


        public string Scheme
        {
            get { return "rabbitmq"; }
        }

        public ILoopbackTransport BuildLoopback(CreateTransportSettings settings)
        {
            EnsureProtocolIsCorrect(settings.Address.Uri);

            var transport = new RabbitMqTransport(settings.Address, GetConnection(settings.Address.Uri));
            return transport;
        }

        public IInboundTransport BuildInbound(CreateTransportSettings settings)
        {
            throw new NotImplementedException();
        }

        public IOutboundTransport BuildOutbound(CreateTransportSettings settings)
        {
            throw new NotImplementedException();
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
            Uri rabbitMqAddress =
                new UriBuilder("amqp-{0}-{1}".FormatWith(_protocol.MajorVersion, _protocol.MinorVersion), address.Host,
                               _protocol.DefaultPort).Uri;
            return _factory.CreateConnection();
        }
        
        public void PurgeExistingMessagesIfRequested(CreateTransportSettings settings)
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
}