// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Linq;
    using BusConfigurators;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using NUnit.Framework;
    using TestFramework;

    [TestFixture]
    public class Bytes_Specs :
        Given_a_rabbitmq_bus
    {
        A _sent;
        Future<A> _received;

        protected override void ConfigureServiceBus(Uri uri, ServiceBusConfigurator configurator)
        {
            base.ConfigureServiceBus(uri, configurator);

            _received = new Future<A>();

            configurator.Subscribe(x => { x.Handler<A>(msg => _received.Complete(msg)); });
        }

        class A
        {
            public byte[] Contents { get; set; }

            public bool Equals(A other)
            {
                if (ReferenceEquals(null, other))
                    return false;
                if (ReferenceEquals(this, other))
                    return true;
                return other.Contents.SequenceEqual(Contents);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                if (obj.GetType() != typeof(A))
                    return false;
                return Equals((A)obj);
            }

            public override int GetHashCode()
            {
                return (Contents != null ? Contents.GetHashCode() : 0);
            }
        }

        [Test]
        public void Should_receive_byte_array()
        {
            _sent = new A
                {
                    Contents = new byte[] {0x56, 0x34, 0xf3}
                };

            LocalBus.Endpoint.Send(_sent);

            _received.WaitUntilCompleted(8.Seconds()).ShouldBeTrue();
        }

        [Test]
        public void Should_receive_byte_array_of_bigness()
        {
            Random random = new Random();
            byte[] bytes = new byte[512];
            for (int i = 0; i < 512; i++)
            {
                bytes[i] = (byte)random.Next(255);
            }
            _sent = new A
                {
                    Contents = bytes
                };

            LocalBus.Endpoint.Send(_sent);

            _received.WaitUntilCompleted(8.Seconds()).ShouldBeTrue();
            _received.Value.ShouldEqual(_sent);
        }
    }
}