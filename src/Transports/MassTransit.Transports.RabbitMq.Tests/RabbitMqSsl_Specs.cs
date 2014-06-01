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
namespace MassTransit.Transports.RabbitMq.Tests
{
    using System;
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    using Magnum.TestFramework;
    using NUnit.Framework;
    using RabbitMQ.Client;


    [Scenario, Explicit]
    public class When_connecting_to_a_rabbit_mq_server_using_ssl
    {
        IServiceBus _bus;

        [When]
        public void Connecting_to_a_rabbit_mq_server_using_ssl_and_with_client_certificate()
        {
            var inputAddress = new Uri("rabbitmq://localhost:5671/test_queue");

            _bus = ServiceBusFactory.New(c =>
                {
                    c.ReceiveFrom(inputAddress);
                    c.UseRabbitMq(r =>
                        {
                            r.ConfigureHost(inputAddress, h =>
                                {
                                    h.UseSsl(s =>
                                        {
                                            s.SetServerName(Dns.GetHostName());
                                            s.SetCertificatePath("client.p12");
                                            s.SetCertificatePassphrase("Passw0rd");
                                        });
                                });
                        });
                });
        }

        [When]
        public void Connecting_to_a_rabbit_mq_server_using_ssl_and_with_an_explicitly_loaded_client_certificate()
        {
            var inputAddress = new Uri("rabbitmq://localhost:5671/test_queue");
            var cert = new X509Certificate2("client.p12", "Passw0rd", X509KeyStorageFlags.MachineKeySet);
            _bus = ServiceBusFactory.New(c =>
            {
                c.ReceiveFrom(inputAddress);
                c.UseRabbitMq(r =>
                {
                    r.ConfigureHost(inputAddress, h =>
                    {
                        h.UseSsl(s =>
                        {
                            s.SetServerName(Dns.GetHostName());
                            s.SetCertificates(cert);
                        });
                    });
                });
            });
        }

        [When]
        public void Connecting_to_a_rabbit_mq_server_using_ssl_without_client_certificate()
        {
            var inputAddress = new Uri("rabbitmq://localhost:5671/test_queue");

            _bus = ServiceBusFactory.New(c =>
                {
                    c.ReceiveFrom(inputAddress);
                    c.UseRabbitMq(
                        r =>
                            {
                                r.ConfigureHost(inputAddress,
                                    h => { h.UseSsl(s => { s.SetClientCertificateRequired(false); }); });
                            });
                });
        }

        [When]
        public void Connecting_to_a_rabbit_mq_server_using_ssl_client_certificate_authentication()
        {
            var inputAddress = new Uri("rabbitmq://localhost:5671/test_queue");

            _bus = ServiceBusFactory.New(c =>
            {
                c.ReceiveFrom(inputAddress);
                c.UseRabbitMq(r =>
                {
                    r.ConfigureHost(inputAddress, h =>
                    {
                        h.UseSsl(s =>
                        {
                            s.SetServerName(Dns.GetHostName());
                            s.SetCertificatePath("client.p12");
                            s.SetCertificatePassphrase("Passw0rd");
                            s.SetAuthMechanisms(new ExternalMechanismFactory());
                        });
                    });
                });
            });
        }

        [Finally]
        public void Finally()
        {
            _bus.Dispose();
        }

        [Then, Explicit]
        public void Should_connect_to_the_queue()
        {
        }
    }
}