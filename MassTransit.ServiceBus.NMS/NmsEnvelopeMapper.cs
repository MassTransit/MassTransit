/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.

namespace MassTransit.ServiceBus.NMS
{
	using System.IO;
	using System.Runtime.Serialization;
	using System.Runtime.Serialization.Formatters.Binary;
	using Apache.NMS;
	using Apache.NMS.ActiveMQ.Commands;
	using Internal;
	using MessageId=MassTransit.ServiceBus.Util.MessageId;

	public class NmsEnvelopeMapper :
		IEnvelopeMapper<IMessage>
	{
        private static readonly IFormatter _formatter = new BinaryFormatter();
		private readonly ISession _session;

		public NmsEnvelopeMapper(ISession session)
		{
			_session = session;
		}

		#region IEnvelopeMapper<IMessage> Members

		public IMessage ToMessage(IEnvelope envelope)
		{
			IBytesMessage bm = _session.CreateBytesMessage();

			if (envelope.Messages != null && envelope.Messages.Length > 0)
			{
				MemoryStream mem = new MemoryStream();
				_formatter.Serialize(mem, envelope.Messages);

				bm.Content = new byte[mem.Length];
				mem.Seek(0, SeekOrigin.Begin);
				mem.Read(bm.Content, 0, (int) mem.Length);
			}

			if (envelope.ReturnEndpoint != null)
			{
				// TODO Need queue return path
				//envelope.ReturnEndpoint.Uri.ToString()
				//3 is a magic number that = mq queue
				IDestination d = ActiveMQDestination.CreateDestination(3, _session.GetQueue("published_queue").QueueName);
				bm.NMSReplyTo = d;
			}

			if (envelope.TimeToBeReceived < NMSConstants.defaultTimeToLive)
				bm.NMSTimeToLive = envelope.TimeToBeReceived;

			if (!string.IsNullOrEmpty(envelope.Label))
				bm.Properties["NMSXGroupID"] = envelope.Label;

			bm.NMSPersistent = envelope.Recoverable;

			if (envelope.CorrelationId != MessageId.Empty)
				bm.NMSCorrelationID = envelope.CorrelationId;

			return bm;
		}

		public IEnvelope ToEnvelope(IMessage message)
		{
			//    IMessageQueueEndpoint returnAddress = (msg.ResponseQueue != null) ? new MessageQueueEndpoint(msg.ResponseQueue) : null;

			IEnvelope e = new Envelope();

			//    if (string.IsNullOrEmpty(msg.Id))
			//    {
			//        e.Id = MessageId.Empty;
			//    }
			//    else
			//    {
			//        e.Id = msg.Id;
			//    }

			//    if (string.IsNullOrEmpty(msg.CorrelationId))
			//    {
			//        e.CorrelationId = MessageId.Empty;
			//    }
			//    else
			//    {
			//        e.CorrelationId = msg.CorrelationId;
			//    }

			e.TimeToBeReceived = message.NMSTimeToLive;
			e.Recoverable = message.NMSPersistent;

			if (message.Properties.Contains("NMSXGroupID"))
				e.Label = message.Properties["NMSXGroupID"].ToString();

			if (message is IBytesMessage)
			{
				IBytesMessage bm = (IBytesMessage) message;

				MemoryStream mem = new MemoryStream(bm.Content, false);

				MassTransit.ServiceBus.IMessage[] messages =
					_formatter.Deserialize(mem) as MassTransit.ServiceBus.IMessage[];
				e.Messages = messages ?? new MassTransit.ServiceBus.IMessage[] {};
			}


			//    if (e.Id != MessageId.Empty)
			//    {
			//        e.SentTime = msg.SentTime;
			//        e.ArrivedTime = msg.ArrivedTime;
			//    }


			return e;
		}

		#endregion
	}
}