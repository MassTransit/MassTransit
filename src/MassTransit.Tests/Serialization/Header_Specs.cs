// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests.Serialization
{
    using System;
    using Magnum.Extensions;
    using MassTransit.Serialization;
    using NUnit.Framework;
    using TestFramework.Messages;


    public abstract class Setting_the_message_expiration<TSerializer>
        : SerializationSpecificationBase<TSerializer>
        where TSerializer : IMessageSerializer, new()
    {
        [Test]
        public void Should_not_impact_others_if_not_set()
        {
            VerifyMessageHeaderIsPassed(x =>
            {
            }, x =>
            {
            });
        }

        [Test]
        public void Should_pass_the_network_header()
        {
            string network = "Superman";

            VerifyMessageHeaderIsPassed(x => x.SetNetwork(network), x =>
            {
                Assert.AreEqual(network, x.Network);
            });
        }

        [Test]
        public void Should_pass_the_destination_address()
        {
            var address = new Uri("loopback://localhost/queue_name");

            VerifyMessageHeaderIsPassed(x => x.SetDestinationAddress(address),
                x =>
                {
                    Assert.AreEqual(address, x.DestinationAddress);
                });
        }

        [Test]
        public void Should_pass_the_source_address()
        {
            var address = new Uri("loopback://localhost/queue_name");

            VerifyMessageHeaderIsPassed(x => x.SetSourceAddress(address), x =>
            {
                Assert.AreEqual(address, x.SourceAddress);
            });
        }

        [Test]
        public void Should_pass_the_response_address()
        {
            var address = new Uri("loopback://localhost/queue_name");

            VerifyMessageHeaderIsPassed(x => x.SetResponseAddress(address), x =>
            {
                Assert.AreEqual(address, x.ResponseAddress);
            });
        }

        [Test]
        public void Should_pass_the_fault_address()
        {
            var address = new Uri("loopback://localhost/queue_name");

            VerifyMessageHeaderIsPassed(x => x.SetFaultAddress(address), x =>
            {
                Assert.AreEqual(address, x.FaultAddress);
            });
        }

        [Test]
        public void Should_carry_through_the_message_headers()
        {
            DateTime now = 5.Minutes().FromNow();

            var expiration = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond);

//			VerifyMessageHeaderIsPassed(x => x.ExpiresAt(expiration),
//				x => { Assert.AreEqual(expiration.ToUniversalTime(), x.ExpirationTime); });
        }

        void VerifyMessageHeaderIsPassed(Action<ISendContext<PingMessage>> setHeaderAction,
            Action<IConsumeContext> checkHeaderAction)
        {
//			byte[] data;
//			var serializer = new XmlMessageSerializer();
//
//			var message = new PingMessage();
//
//			using (var output = new MemoryStream())
//			{
//				var sendContext = new OldSendContext<PingMessage>(message);
//				setHeaderAction(sendContext);
//
//				serializer.Serialize(output, sendContext);
//
//				data = output.ToArray();
//			}
//
//			//Trace.WriteLine(Encoding.UTF8.GetString(data));
//
//			using (var input = new MemoryStream(data))
//			{
//				var receiveContext = OldReceiveContext.FromBodyStream(input);
//				serializer.Deserialize(receiveContext);
//
//				checkHeaderAction(receiveContext);
//			}
        }
    }


    [TestFixture]
    public class WhenUsingCustomXmlAndHeaders :
        Setting_the_message_expiration<XmlMessageSerializer>
    {
    }


    [TestFixture]
    public class WhenUsingBinaryAndHeaders :
        Setting_the_message_expiration<BinaryMessageSerializer>
    {
    }


    [TestFixture]
    public class WhenUsingJsonAndHeaders :
        Setting_the_message_expiration<JsonMessageSerializer>
    {
    }


    [TestFixture]
    public class WhenUsingBsonAndHeaders :
        Setting_the_message_expiration<BsonMessageSerializer>
    {
    }
}