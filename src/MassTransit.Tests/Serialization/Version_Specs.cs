// Copyright 2007-2008 The Apache Software Foundation.
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
	using System.IO;
	using System.Text;
	using Context;
	using Magnum.TestFramework;
	using MassTransit.Serialization;
	using NUnit.Framework;

	[TestFixture]
	public class Deserializing_a_message_with_a_different_clr_version
	{
		[Test]
		public void Should_not_cause_fatal_explosions_of_the_fiery_death_kind()
		{
			var serializer = new VersionOneXmlMessageSerializer();
			using (var bodyStream = new MemoryStream(Encoding.UTF8.GetBytes(Version4Message)))
			{
				var receiveContext = ReceiveContext.FromBodyStream(bodyStream);
				serializer.Deserialize(receiveContext);

				IConsumeContext<ComplaintAdded> context;
				receiveContext.TryGetContext<ComplaintAdded>(out context).ShouldBeTrue();

				context.ShouldNotBeNull();
			}
		}

		[Test]
		public void Should_handle_the_uri_type()
		{
			var serializer = new VersionOneXmlMessageSerializer();
			using (var bodyStream = new MemoryStream(Encoding.UTF8.GetBytes(AnotherVersion4Message)))
			{
				var receiveContext = ReceiveContext.FromBodyStream(bodyStream);
				serializer.Deserialize(receiveContext);

				IConsumeContext<ComplaintAdded> context;
				receiveContext.TryGetContext<ComplaintAdded>(out context).ShouldBeTrue();

				context.ShouldNotBeNull();
			}
		}

		private const string Version4Message = "<?xml version=\"1.0\" encoding=\"utf-8\"?><x:XmlMessageEnvelope xmlns:x=\"MassTransit.Serialization.XmlMessageEnvelope, MassTransit\" xmlns:m=\"MassTransit.Tests.Serialization.ComplaintAdded, MassTransit.Tests\" xmlns:a=\"MassTransit.Tests.Serialization.User, MassTransit.Tests\" xmlns:s=\"System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\" xmlns:d=\"System.DateTime, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\" xmlns:b=\"MassTransit.Tests.Serialization.BusinessArea, MassTransit.Tests\" xmlns:i=\"System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"><m:Message><a:AddedBy><s:Name>Chris</s:Name><s:Email>noone@nowhere.com</s:Email></a:AddedBy><d:AddedAt>2009-09-15T20:12:34.3939205Z</d:AddedAt><s:Subject>No toilet paper</s:Subject><s:Body>There was no toilet paper in the stall, forcing me to use my treasured issue of .NET Developer magazine.</s:Body><b:Area>Appearance</b:Area></m:Message><s:SourceAddress>loopback://localhost/source</s:SourceAddress><s:DestinationAddress>loopback://localhost/destination</s:DestinationAddress><s:ResponseAddress>loopback://localhost/response</s:ResponseAddress><s:FaultAddress>loopback://localhost/fault</s:FaultAddress><i:RetryCount>69</i:RetryCount><s:MessageType>MassTransit.Tests.Serialization.ComplaintAdded, MassTransit.Tests</s:MessageType></x:XmlMessageEnvelope>";

		private const string AnotherVersion4Message = "<?xml version=\"1.0\" encoding=\"utf-8\"?><x:XmlMessageEnvelope xmlns:x=\"MassTransit.Serialization.XmlMessageEnvelope, MassTransit\" xmlns:m=\"MassTransit.Tests.Serialization.ComplaintAdded, MassTransit.Tests\" xmlns:a=\"MassTransit.Tests.Serialization.User, MassTransit.Tests\" xmlns:s=\"System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\" xmlns:u=\"System.Uri, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\" xmlns:d=\"System.DateTime, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\" xmlns:b=\"MassTransit.Tests.Serialization.BusinessArea, MassTransit.Tests\" xmlns:i=\"System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"><m:Message><a:AddedBy><s:Name>Chris</s:Name><s:Email>noone@nowhere.com</s:Email></a:AddedBy><d:AddedAt>2009-09-15T20:12:34.3939205Z</d:AddedAt><s:Subject>No toilet paper</s:Subject><s:Body>There was no toilet paper in the stall, forcing me to use my treasured issue of .NET Developer magazine.</s:Body><b:Area>Appearance</b:Area></m:Message><s:SourceAddress>loopback://localhost/source</s:SourceAddress><s:DestinationAddress>loopback://localhost/destination</s:DestinationAddress><s:ResponseAddress>loopback://localhost/response</s:ResponseAddress><u:FaultAddress>loopback://localhost/fault</u:FaultAddress><i:RetryCount>69</i:RetryCount><s:MessageType>MassTransit.Tests.Serialization.ComplaintAdded, MassTransit.Tests</s:MessageType></x:XmlMessageEnvelope>";
	}
}