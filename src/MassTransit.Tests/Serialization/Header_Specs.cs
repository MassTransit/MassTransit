// Copyright 2007-2010 The Apache Software Foundation.
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
	using System.Diagnostics;
	using System.IO;
	using System.Text;
	using Magnum.Extensions;
	using MassTransit.Internal;
	using MassTransit.Serialization;
	using Messages;
	using NUnit.Framework;


	public abstract class Setting_the_message_expiration<TSerializer> 
		: SerializationSpecificationBase<TSerializer> where TSerializer : IMessageSerializer, new()
	{
		[Test]
		public void Should_not_impact_others_if_not_set()
		{
			VerifyMessageHeaderIsPassed(x => { }, x => { });
		}

		[Test]
		public void Should_carry_through_the_message_headers()
		{
			DateTime expiration = 5.Minutes().FromNow();

			VerifyMessageHeaderIsPassed(x => x.ExpiresAt(expiration), x => { Assert.AreEqual(expiration.ToUniversalTime(), x.ExpirationTime); });
		}

		private void VerifyMessageHeaderIsPassed(Action<IOutboundMessage> setHeaderAction, Action<IInboundMessageHeaders> checkHeaderAction)
		{
			byte[] data;
			var serializer = new XmlMessageSerializer();

			OutboundMessage.Set(setHeaderAction);

			var message = new PingMessage();

			using (MemoryStream output = new MemoryStream())
			{
				serializer.Serialize(output, message);

				data = output.ToArray();
			}

			Trace.WriteLine(OutboundMessage.Headers.MessageType);

			Trace.WriteLine(Encoding.UTF8.GetString(data));

			using (MemoryStream input = new MemoryStream(data))
			{
				serializer.Deserialize(input);

				checkHeaderAction(CurrentMessage.Headers);
			}
		}
	}

    [TestFixture]
    public class WhenUsingCustomXmlAndHeaders :
        Setting_the_message_expiration<CustomXmlMessageSerializer>
    {
    }
    [TestFixture]
    public class WhenUsingDotNotXmlAndHeaders :
        Setting_the_message_expiration<DotNotXmlMessageSerializer>
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
}