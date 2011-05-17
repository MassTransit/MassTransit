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
namespace MassTransit.Serialization
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Context;

	public class Envelope
	{
		Envelope(object message, IEnumerable<Type> messageTypes)
		{
			Headers = new Dictionary<string, string>();
			MessageType = new List<string>(messageTypes.Select(type => new MessageUrn(type).ToString()));
			Message = message;
		}

		protected Envelope()
		{
			Headers = new Dictionary<string, string>();
			MessageType = new List<string>();
		}

		public string ConversationId { get; set; }
		public string CorrelationId { get; set; }
		public string DestinationAddress { get; set; }
		public DateTime? ExpirationTime { get; set; }
		public string FaultAddress { get; set; }
		public IDictionary<string, string> Headers { get; set; }
		public object Message { get; set; }
		public string MessageId { get; set; }
		public IList<string> MessageType { get; set; }
		public string Network { get; set; }
		public string ResponseAddress { get; set; }
		public int RetryCount { get; set; }
		public string SourceAddress { get; set; }

		public static Envelope Create<T>(ISendContext<T> context)
			where T : class
		{
			var envelope = new Envelope(context.Message, typeof (T).GetMessageTypes());
			envelope.SetUsingContext(context);

			return envelope;
		}
	}

	public static class EnvelopeExtensions
	{
		public static void SetUsingEnvelope(this IReceiveContext context, Envelope envelope)
		{
			context.SetSourceAddress(envelope.SourceAddress.ToUriOrNull());
			context.SetDestinationAddress(envelope.DestinationAddress.ToUriOrNull());
			context.SetResponseAddress(envelope.ResponseAddress.ToUriOrNull());
			context.SetFaultAddress(envelope.FaultAddress.ToUriOrNull());
			context.SetNetwork(envelope.Network);
			context.SetRetryCount(envelope.RetryCount);
			if (envelope.ExpirationTime.HasValue)
				context.SetExpirationTime(envelope.ExpirationTime.Value);
		}

		public static void SetUsingContext(this Envelope envelope, ISendContext headers)
		{
			envelope.SourceAddress = headers.SourceAddress.ToStringOrNull() ?? envelope.SourceAddress;
			envelope.DestinationAddress = headers.DestinationAddress.ToStringOrNull() ?? envelope.DestinationAddress;
			envelope.ResponseAddress = headers.ResponseAddress.ToStringOrNull() ?? envelope.ResponseAddress;
			envelope.FaultAddress = headers.FaultAddress.ToStringOrNull() ?? envelope.FaultAddress;
			envelope.Network = headers.Network;
			envelope.RetryCount = headers.RetryCount;
			if (headers.ExpirationTime.HasValue)
				envelope.ExpirationTime = headers.ExpirationTime.Value;
		}

		public static bool IsAllowedMessageType(this Type type)
		{
			if(type.Namespace == null)
				return false;

			if (type.Assembly == typeof(object).Assembly)
				return false;

			if (type.Namespace == "System")
				return false;

			if(type.Namespace.StartsWith("System."))
				return false;

			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(CorrelatedBy<>))
				return false;

			return true;
		}

		public static IEnumerable<Type> GetMessageTypes(this Type messageType)
		{
			yield return messageType;

			Type baseType = messageType.BaseType;
			while ((baseType != null) && baseType.IsAllowedMessageType())
			{
				yield return baseType;

				baseType = baseType.BaseType;
			}

			IEnumerable<Type> interfaces = messageType
				.GetInterfaces()
				.Where(x => x.IsAllowedMessageType());

			foreach (Type interfaceType in interfaces)
			{
				yield return interfaceType;
			}
		}
	}
}
