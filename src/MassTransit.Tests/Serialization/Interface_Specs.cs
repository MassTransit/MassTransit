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
	using System;
	using System.IO;
	using System.Text;
	using Context;
	using MassTransit.Pipeline;
	using MassTransit.Pipeline.Configuration;
	using MassTransit.Pipeline.Inspectors;
	using MassTransit.Serialization;
	using NUnit.Framework;
	using TestConsumers;

	
	public abstract class Deserializing_an_interface<TSerializer> :
	    SerializationSpecificationBase<TSerializer> where TSerializer : IMessageSerializer, new()
	{
		[Test]
		public void Should_create_a_proxy_for_the_interface()
		{
			var user = new UserImpl("Chris", "noone@nowhere.com");
			ComplaintAdded complaint = new ComplaintAddedImpl(user, "No toilet paper", BusinessArea.Appearance)
				{
					Body = "There was no toilet paper in the stall, forcing me to use my treasured issue of .NET Developer magazine."
				};

			TestSerialization(complaint);
		}

		[Test]
		public void Should_deserialize_into_the_proxy_object()
		{
			var serializer = new TSerializer();
			using (var bodyStream = new MemoryStream(Encoding.UTF8.GetBytes(InterfaceBasedMessageXml)))
			{
				object obj = serializer.Deserialize(new ConsumeContext(bodyStream));

				Assert.IsNotNull(obj);
			}
		}

		[Test]
		public void Should_dispatch_an_interface_via_the_pipeline()
		{
			var pipeline = InboundPipelineConfigurator.CreateDefault(null);

			var consumer = new TestMessageConsumer<ComplaintAdded>();

			var unsubscribeAction = pipeline.ConnectInstance(consumer);

			var user = new UserImpl("Chris", "noone@nowhere.com");
			ComplaintAdded complaint = new ComplaintAddedImpl(user, "No toilet paper", BusinessArea.Appearance)
				{
					Body = "There was no toilet paper in the stall, forcing me to use my treasured issue of .NET Developer magazine."
				};

			pipeline.Dispatch(complaint);

	//		PipelineViewer.Trace(pipeline);

			consumer.ShouldHaveReceivedMessage(complaint);
		}

		private const string InterfaceBasedMessageXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><x:XmlMessageEnvelope xmlns:x=\"MassTransit.Serialization.XmlMessageEnvelope, MassTransit, Version=0.5.0.1991, Culture=neutral, PublicKeyToken=null\" xmlns:m=\"MassTransit.Tests.Serialization.ComplaintAdded, MassTransit.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\" xmlns:a=\"MassTransit.Tests.Serialization.User, MassTransit.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\" xmlns:s=\"System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\" xmlns:d=\"System.DateTime, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\" xmlns:b=\"MassTransit.Tests.Serialization.BusinessArea, MassTransit.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\" xmlns:i=\"System.Int32, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"><m:Message><a:AddedBy><s:Name>Chris</s:Name><s:Email>noone@nowhere.com</s:Email></a:AddedBy><d:AddedAt>2009-09-15T20:12:34.3939205Z</d:AddedAt><s:Subject>No toilet paper</s:Subject><s:Body>There was no toilet paper in the stall, forcing me to use my treasured issue of .NET Developer magazine.</s:Body><b:Area>Appearance</b:Area></m:Message><s:SourceAddress>loopback://localhost/source</s:SourceAddress><s:DestinationAddress>loopback://localhost/destination</s:DestinationAddress><s:ResponseAddress>loopback://localhost/response</s:ResponseAddress><s:FaultAddress>loopback://localhost/fault</s:FaultAddress><i:RetryCount>69</i:RetryCount><s:MessageType>MassTransit.Tests.Serialization.ComplaintAdded, MassTransit.Tests</s:MessageType></x:XmlMessageEnvelope>";
	}

    [TestFixture]
    public class WhenUsingCustomXml :
        Deserializing_an_interface<XmlMessageSerializer>
    {
        
    }

    [TestFixture][Ignore("the built in dot net xml serializer doesn't support this feature")]
    public class WhenUsingDotNotXml :
        Deserializing_an_interface<DotNotXmlMessageSerializer>
    {
    }

    [TestFixture][Ignore("the built in binary serializer doesn't support this feature")]
    public class WhenUsingBinary :
        Deserializing_an_interface<BinaryMessageSerializer>
    {
    }

    [TestFixture][Ignore("the current impl of json serilaization doesn't support this feature")]
    public class WhenUsingJson :
        Deserializing_an_interface<JsonMessageSerializer>
    {
        
    }

	public interface ComplaintAdded
	{
		User AddedBy { get; }

		DateTime AddedAt { get; }

		string Subject { get; }

		string Body { get; }

		BusinessArea Area { get; }
	}

	public enum BusinessArea
	{
		Unknown = 0,
		Appearance,
		Courtesy,
	}

	public interface User
	{
		string Name { get; }
		string Email { get; }
	}

	public class UserImpl : User
	{
		public UserImpl(string name, string email)
		{
			Name = name;
			Email = email;
		}

		protected UserImpl()
		{
		}

		public string Name { get; set; }

		public string Email { get; set; }

		public bool Equals(User other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Name, Name) && Equals(other.Email, Email);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if(!typeof(User).IsAssignableFrom(obj.GetType())) return false;
			return Equals((User) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Name != null ? Name.GetHashCode() : 0)*397) ^ (Email != null ? Email.GetHashCode() : 0);
			}
		}
	}

	public class ComplaintAddedImpl :
		ComplaintAdded
	{
		public ComplaintAddedImpl(User addedBy, string subject, BusinessArea area)
		{
			AddedAt = DateTime.UtcNow;

			AddedBy = addedBy;
			Subject = subject;
			Area = area;
			Body = string.Empty;
		}

		protected ComplaintAddedImpl()
		{
		}

		public User AddedBy { get; set; }

		public DateTime AddedAt { get; set; }

		public string Subject { get; set; }

		public string Body { get; set; }

		public BusinessArea Area { get; set; }

		public bool Equals(ComplaintAdded other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return AddedBy.Equals(other.AddedBy) && other.AddedAt.Equals(AddedAt) && Equals(other.Subject, Subject) && Equals(other.Body, Body) && Equals(other.Area, Area);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (!typeof(ComplaintAdded).IsAssignableFrom(obj.GetType())) return false;
			return Equals((ComplaintAdded)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int result = (AddedBy != null ? AddedBy.GetHashCode() : 0);
				result = (result*397) ^ AddedAt.GetHashCode();
				result = (result*397) ^ (Subject != null ? Subject.GetHashCode() : 0);
				result = (result*397) ^ (Body != null ? Body.GetHashCode() : 0);
				result = (result*397) ^ Area.GetHashCode();
				return result;
			}
		}
	}
}