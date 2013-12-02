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
    using Magnum.TestFramework;
    using NUnit.Framework;


    [Scenario]
    public class GivenAVHostAddress
    {
        public Uri Uri = new Uri("rabbitmq://some_server/thehost/queue");
        RabbitMqEndpointAddress _addr;

        [When]
        public void WhenParsed()
        {
            _addr = RabbitMqEndpointAddress.Parse(Uri);
        }

        [Then]
        public void TheHost()
        {
            _addr.ConnectionFactory.VirtualHost.ShouldEqual("thehost");
        }

        [Then]
        public void TheQueue()
        {
            _addr.Name.ShouldEqual("queue");
        }

        [Then]
        public void Rebuilt()
        {
//            _addr.RebuiltUri.ShouldEqual(new Uri("rabbitmq://guest:guest@some_server:5432/thehost/queue"));
        }

        [Then]
        public void RelativeQueue()
        {
            _addr.ForQueue("anotherone")
                 .Uri.ToString().ShouldEqual("rabbitmq://some_server/thehost/anotherone");
        }

        [Then]
        public void ShouldNotBeHa()
        {
            _addr.QueueArguments().ShouldBeNull();
        }

        [Then]
        public void ShouldNotHaveATtl()
        {
            _addr.QueueArguments().ShouldBeNull();
        }
    }


    [Scenario]
    public class GivenAnAddressWithUnderscore
    {
        public Uri Uri = new Uri("rabbitmq://some_server/thehost/the_queue");
        RabbitMqEndpointAddress _addr;

        [When]
        public void WhenParsed()
        {
            _addr = RabbitMqEndpointAddress.Parse(Uri);
        }

        [Then]
        public void TheQueue()
        {
            _addr.Name.ShouldEqual("the_queue");
        }

        [Then]
        public void Rebuilt()
        {
            //          _addr.RebuiltUri.ShouldEqual(new Uri("rabbitmq://guest:guest@some_server:5432/thehost/the_queue"));
        }
    }


    [Scenario]
    public class GivenAnAddressWithPeriod
    {
        public Uri Uri = new Uri("rabbitmq://some_server/thehost/the.queue");
        RabbitMqEndpointAddress _addr;

        [When]
        public void WhenParsed()
        {
            _addr = RabbitMqEndpointAddress.Parse(Uri);
        }

        [Then]
        public void TheQueue()
        {
            _addr.Name.ShouldEqual("the.queue");
        }

        [Then]
        public void Rebuilt()
        {
            //    _addr.RebuiltUri.ShouldEqual(new Uri("rabbitmq://guest:guest@some_server:5432/thehost/the.queue"));
        }
    }


    [Scenario]
    public class GivenAnAddressWithColon
    {
        public Uri Uri = new Uri("rabbitmq://some_server/thehost/the:queue");
        RabbitMqEndpointAddress _addr;

        [When]
        public void WhenParsed()
        {
            _addr = RabbitMqEndpointAddress.Parse(Uri);
        }

        [Then]
        public void TheQueue()
        {
            _addr.Name.ShouldEqual("the:queue");
        }

        [Then]
        public void Rebuilt()
        {
            //    _addr.RebuiltUri.ShouldEqual(new Uri("rabbitmq://guest:guest@some_server:5432/thehost/the:queue"));
        }
    }


    [Scenario]
    public class GivenAnAddressWithSlash
    {
        public Uri Uri = new Uri("rabbitmq://some_server/thehost/the/queue");
        RabbitMqEndpointAddress _addr;

        [When]
        public void WhenParsed()
        {
        }

        [Then, ExpectedException(typeof(RabbitMqAddressException))]
        
        public void TheQueue()
        {
            _addr = RabbitMqEndpointAddress.Parse(Uri);
        }
    }


    [Scenario]
    public class GivenANonVHostAddress
    {
        public Uri Uri = new Uri("rabbitmq://some_server/the_queue");
        RabbitMqEndpointAddress _addr;

        [When]
        public void WhenParsed()
        {
            _addr = RabbitMqEndpointAddress.Parse(Uri);
        }

        [Then]
        public void TheHost()
        {
            _addr.ConnectionFactory.VirtualHost.ShouldEqual("/");
        }

        [Then]
        public void TheQueue()
        {
            _addr.Name.ShouldEqual("the_queue");
        }
    }


    [Scenario]
    public class GivenAPortedAddress
    {
        public Uri Uri = new Uri("rabbitmq://some_server:12/the_queue");
        RabbitMqEndpointAddress _addr;

        [When]
        public void WhenParsed()
        {
            _addr = RabbitMqEndpointAddress.Parse(Uri);
        }

        [Then]
        public void TheHost()
        {
            _addr.ConnectionFactory.VirtualHost.ShouldEqual("/");
        }

        [Then]
        public void ThePort()
        {
            _addr.ConnectionFactory.Port.ShouldEqual(12);
        }

        [Then]
        public void Rebuilt()
        {
            //    _addr.RebuiltUri.ShouldEqual(new Uri(@"rabbitmq://guest:guest@some_server:12/the_queue"));
        }
    }


    [Scenario]
    public class GivenANonPortedAddress
    {
        public Uri Uri = new Uri("rabbitmq://some_server/the_queue");
        RabbitMqEndpointAddress _addr;

        [When]
        public void WhenParsed()
        {
            _addr = RabbitMqEndpointAddress.Parse(Uri);
        }

        [Then]
        public void TheHost()
        {
            _addr.ConnectionFactory.VirtualHost.ShouldEqual("/");
        }

        [Then]
        public void ThePort()
        {
            _addr.ConnectionFactory.Port.ShouldEqual(5672);
        }
    }

    [Scenario]
    public class GivenAUserNameUrl
    {
        public Uri Uri = new Uri("rabbitmq://dru:mt@some_server/thehost/the_queue");
        RabbitMqEndpointAddress _addr;

        [When]
        public void WhenParsed()
        {
            _addr = RabbitMqEndpointAddress.Parse(Uri);
        }

        [Then]
        public void TheUsername()
        {
            _addr.ConnectionFactory.UserName.ShouldEqual("dru");
        }

        [Then]
        public void ThePassword()
        {
            _addr.ConnectionFactory.Password.ShouldEqual("mt");
        }

        [Then]
        public void Rebuilt()
        {
            //_addr.RebuiltUri.ShouldEqual(new Uri("rabbitmq://dru:mt@some_server:5432/thehost/the_queue"));
        }
    }


    [Scenario]
    public class GivenAHighAvailableQueue
    {
        RabbitMqEndpointAddress _addr;
        public string uri = "rabbitmq://localhost/somequeue?ha=true";

        [When]
        public void WhenParsed()
        {
            _addr = RabbitMqEndpointAddress.Parse(uri);
        }

        [Then]
        public void TheQueueArguments()
        {
            _addr.QueueArguments().ShouldNotBeNull();
        }

        [Then]
        public void HighAvailabilityQueue()
        {
            _addr.QueueArguments()["x-ha-policy"].ShouldEqual("all");
        }

        [Then]
        public void ShouldNotHaveATtl()
        {
            _addr.QueueArguments().ContainsKey("x-message-ttl").ShouldBeFalse();
        }

        [Then]
        public void TheQueueName()
        {
            _addr.Name.ShouldEqual("somequeue");
        }

        [Then]
        public void should_not_use_query_string_of_uri()
        {
            _addr.ForQueue("anotherone").Uri.ToString().ShouldEqual("rabbitmq://localhost/anotherone");
            _addr.ForQueue("anotherone").Name.ShouldEqual("anotherone");
        }
    }

    [Scenario]
    public class Given_a_prefetch_count
    {
        RabbitMqEndpointAddress _addr;
        public string uri = "rabbitmq://localhost/somequeue?ha=true&prefetch=32";

        [When]
        public void WhenParsed()
        {
            _addr = RabbitMqEndpointAddress.Parse(uri);
        }

        [Then]
        public void TheQueueArguments()
        {
            _addr.QueueArguments().ShouldNotBeNull();
        }

        [Then]
        public void HighAvailabilityQueue()
        {
            _addr.QueueArguments()["x-ha-policy"].ShouldEqual("all");
        }

        [Then]
        public void Should_have_the_prefetch_count_on_the_address()
        {
            _addr.PrefetchCount.ShouldEqual((ushort)32);
        }

        [Then]
        public void TheQueueName()
        {
            _addr.Name.ShouldEqual("somequeue");
        }

        [Then]
        public void should_not_use_query_string_of_uri()
        {
            _addr.ForQueue("anotherone").Uri.ToString().ShouldEqual("rabbitmq://localhost/anotherone");
            _addr.ForQueue("anotherone").Name.ShouldEqual("anotherone");
        }
    }

    [Scenario]
    public class Given_a_temporary_queue_was_requested
    {
        RabbitMqEndpointAddress _addr;
        public string uri = "rabbitmq://localhost/*?temporary=true";

        [When]
        public void WhenParsed()
        {
            _addr = RabbitMqEndpointAddress.Parse(uri);
        }

        [Then]
        public void TheQueueArguments()
        {
            _addr.QueueArguments().ShouldBeNull();
        }

        [Then]
        public void Should_not_be_durable()
        {
            _addr.Durable.ShouldBeFalse();
        }

        [Then]
        public void Should_be_exclusive_to_the_consumer()
        {
            _addr.Exclusive.ShouldBeTrue();
        }

        [Then]
        public void Should_be_auto_delete()
        {
            _addr.AutoDelete.ShouldBeTrue();
        }

        [Then]
        public void TheQueueName()
        {
            var guid = new Guid(_addr.Name);
            Assert.AreNotEqual(Guid.Empty, guid);
        }
    }


    [Scenario]
    public class GivenATtl
    {
        RabbitMqEndpointAddress _addr;
        public string uri = "rabbitmq://localhost/somequeue?ttl=20";

        [When]
        public void WhenParsed()
        {
            _addr = RabbitMqEndpointAddress.Parse(uri);
        }

        [Then]
        public void TheQueueArguments()
        {
            _addr.QueueArguments().ShouldNotBeNull();
        }

        [Then]
        public void TtlQueue()
        {
            _addr.QueueArguments()["x-message-ttl"].ShouldEqual(20);
        }


        [Then]
        public void ShouldNotBeHa()
        {
            _addr.QueueArguments().ContainsKey("x-ha-policy").ShouldBeFalse();
        }

        [Then]
        public void TheQueueName()
        {
            _addr.Name.ShouldEqual("somequeue");
        }

        [Then]
        public void should_not_use_query_string_of_uri()
        {
            _addr.ForQueue("anotherone").Uri.ToString().ShouldEqual("rabbitmq://localhost/anotherone");
            _addr.ForQueue("anotherone").Name.ShouldEqual("anotherone");
        }
    }
}