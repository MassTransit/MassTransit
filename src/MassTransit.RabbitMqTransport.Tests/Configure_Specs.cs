// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using GreenPipes;
    using NUnit.Framework;


    [TestFixture]
    public class Configure_Specs
    {
        [Test]
        public void Should_fail_on_no_hosts()
        {
            var exception = Assert.Throws<ConfigurationException>(() =>
            {
                Bus.Factory.CreateUsingRabbitMq(x =>
                {
                });
            });


            Console.WriteLine(string.Join(Environment.NewLine, exception.Result.Results));
        }

        [Test]
        public void Should_fail_with_invalid_middleware()
        {
            var exception = Assert.Throws<ConfigurationException>(() =>
            {
                Bus.Factory.CreateUsingRabbitMq(x =>
                {
                    var host = x.Host(new Uri("rabbitmq://[::1]/test/"), h =>
                    {
                    });

                    x.UseRetry(r =>
                    {
                    });
                });
            });


            Console.WriteLine(string.Join(Environment.NewLine, exception.Result.Results));

        }
        [Test]
        public void Should_fail_when_late_configuration_happens()
        {
            var exception = Assert.Throws<ConfigurationException>(() =>
            {
                Bus.Factory.CreateUsingRabbitMq(x =>
                {
                    var host = x.Host(new Uri("rabbitmq://[::1]/test/"), h =>
                    {
                    });

                    x.ReceiveEndpoint(host, "input_queue", e =>
                    {
                        var inputAddress = e.InputAddress;

                        e.Durable = false;
                        e.AutoDelete = true;
                    });
                });
            });


            Console.WriteLine(string.Join(Environment.NewLine, exception.Result.Results));
        }

        [Test]
        public void Should_fail_with_invalid_middleware_on_endpoint()
        {
            var exception = Assert.Throws<ConfigurationException>(() =>
            {
                Bus.Factory.CreateUsingRabbitMq(x =>
                {
                    var host = x.Host(new Uri("rabbitmq://[::1]/test/"), h =>
                    {
                    });

                    x.ReceiveEndpoint(host, "input_queue", e =>
                    {
                        e.UseRetry(r =>
                        {
                        });
                    });
                });
            });


            Console.WriteLine(string.Join(Environment.NewLine, exception.Result.Results));
        }

        [Test]
        public void Should_fail_with_empty_queue_name()
        {
            var exception = Assert.Throws<ConfigurationException>(() =>
            {
                Bus.Factory.CreateUsingRabbitMq(x =>
                {
                    var host = x.Host(new Uri("rabbitmq://[::1]/test/"), h =>
                    {
                    });

                    x.OverrideDefaultBusEndpointQueueName("");
                });
            });


            Console.WriteLine(string.Join(Environment.NewLine, exception.Result.Results));
        }

        [Test]
        public void Should_fail_with_invalid_queue_name()
        {
            var exception = Assert.Throws<ConfigurationException>(() =>
            {
                Bus.Factory.CreateUsingRabbitMq(x =>
                {
                    var host = x.Host(new Uri("rabbitmq://[::1]/test/"), h =>
                    {
                    });

                    x.ReceiveEndpoint(host, "0(*!)@((*#&!(*&@#/", e =>
                    {
                    });
                });
            });


            Console.WriteLine(string.Join(Environment.NewLine, exception.Result.Results));
        }

        [Test]
        public void Should_not_fail_with_warnings()
        {
            Bus.Factory.CreateUsingRabbitMq(x =>
            {
                var host = x.Host(new Uri("rabbitmq://[::1]/test/"), h =>
                {
                });

                x.ReceiveEndpoint(host, "input_queue", e =>
                {
                    e.PurgeOnStartup = true;
                });
            });
        }
    }
}