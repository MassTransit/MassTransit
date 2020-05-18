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
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;
    using Util;


    [TestFixture]
    public class Serializing_an_object_in_a_header :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_properly_serialize_and_deserialize()
        {
            await InputQueueSendEndpoint.Send(new PingMessage(), context =>
            {
                context.Headers.Set("Claims-Identity", new ClaimsIdentityProxy
                {
                    IdentityType = "AAD:Claims",
                    IdentityId = 27,
                    Claims = new[]
                    {
                        new ClaimProxy {Issuer = "Azure", Type="User", Value="37" },
                        new ClaimProxy {Issuer = "Azure", Type="User", Value="457" },
                        new ClaimProxy {Issuer = "Azure", Type="User", Value="451" },
                    }
                });
            });

            var consumeContext = await _handled;

            var identity = await _header.Task;

            Assert.AreEqual(27, identity.IdentityId);
            Assert.AreEqual("AAD:Claims", identity.IdentityType);
            Assert.AreEqual(3, identity.Claims.Length);
            Assert.AreEqual("Azure", identity.Claims[0].Issuer);
        }

        Task<ConsumeContext<PingMessage>> _handled;
        TaskCompletionSource<ClaimsIdentity> _header;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _header = GetTask<ClaimsIdentity>();

            _handled = Handler<PingMessage>(configurator, context =>
            {
                _header.TrySetResult(context.Headers.Get<ClaimsIdentity>("Claims-Identity"));

                return TaskUtil.Completed;
            });
        }


        public interface ClaimsIdentity
        {
            string IdentityType { get; }
            int IdentityId { get; }
            Claim[] Claims { get; }
        }


        public interface Claim
        {
            string Type { get;  }
             string Value { get;  }
             string ValueType { get;  }
             string Issuer { get;  }
        }


        public class ClaimProxy :
            Claim
        {
            public string Type { get; set; }
            public string Value { get; set; }
            public string ValueType { get; set; }
            public string Issuer { get; set; }
        }


        class ClaimsIdentityProxy :
            ClaimsIdentity
        {
            public string IdentityType { get; set; }
            public int IdentityId { get; set; }
            public Claim[] Claims { get; set; }
        }
    }


    [TestFixture]
    public class Header_properties_of_unsupported_values :
        RabbitMqTestFixture
    {
        Task<ConsumeContext<PingMessage>> _handled;
        DateTime _now;
        DateTimeOffset _later;

        [Test]
        public async Task Should_support_date_time()
        {
            var context = await _handled;

            Assert.AreEqual(_now, context.Headers.Get("Now", default(DateTime?)));

            Assert.AreEqual(_later, context.Headers.Get("Later", default(DateTimeOffset?)));

        }

        [OneTimeSetUp]
        public void Setup()
        {
            InputQueueSendEndpoint.Send(new PingMessage(), context =>
            {
                _now = DateTime.UtcNow;
                context.Headers.Set("Now", _now);

                _later = DateTimeOffset.Now;
                context.Headers.Set("Later", _later);
            });
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            base.ConfigureRabbitMqReceiveEndpoint(configurator);

            _handled = Handled<PingMessage>(configurator);
        }
    }
}