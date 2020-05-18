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
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;


    [TestFixture]
    public class Bytes_Specs :
        RabbitMqTestFixture
    {
        Task<ConsumeContext<A>> _received;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _received = Handled<A>(configurator);
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
        public async Task Should_receive_byte_array_of_bigness()
        {
            var random = new Random();
            var bytes = new byte[512];
            for (int i = 0; i < 512; i++)
                bytes[i] = (byte)random.Next(255);

            var sent = new A
            {
                Contents = bytes
            };
            await InputQueueSendEndpoint.Send(sent);

            ConsumeContext<A> context = await _received;

            context.Message.ShouldBe(sent);
        }
    }
}