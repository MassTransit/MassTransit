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
namespace MassTransit.TestFramework.Transports
{
	using System;
	using System.Transactions;
	using Context;
	using MassTransit.Transports;
	using Messages;
	using NUnit.Framework;
	using Serialization;

	[TestFixture]
	public abstract class NonTransactionalTransportSendingContract<TTransportFactory>
		where TTransportFactory : ITransportFactory
	{
		[SetUp]
		public void SetUp()
		{
			_serializer = new XmlMessageSerializer();
			_transport = _factory.BuildOutbound(new TransportSettings(new EndpointAddress(Address)));
		}

		[TearDown]
		public void TearDown()
		{
			_transport.Dispose();
		}

		IOutboundTransport _transport;
		readonly ITransportFactory _factory;
		IMessageSerializer _serializer;

		public Uri Address { get; set; }
		public Uri AddressToCheck { get; set; }

		protected NonTransactionalTransportSendingContract(Uri uri, TTransportFactory factory)
			: this(uri, uri, factory)
		{
		}

		protected NonTransactionalTransportSendingContract(Uri sendingUri, Uri checkingUri, TTransportFactory factory)
		{
			Address = sendingUri;
			AddressToCheck = checkingUri;

			_factory = factory;
		}

		public void VerifyMessageIsInQueue()
		{
			IInboundTransport transport = _factory.BuildInbound(new TransportSettings(new EndpointAddress(AddressToCheck)));
			transport.ShouldContain<DeleteMessage>(_serializer);
		}

		void SendMessage()
		{
			var context = new SendContext<DeleteMessage>(new DeleteMessage());
			context.SetBodyWriter(stream => _serializer.Serialize(stream, context));

			_transport.Send(context);
		}

		[Test]
		public void While_sending_it_should_perisist_even_on_rollback()
		{
			using (new TransactionScope())
			{
				SendMessage();

				//no complete
			}

			VerifyMessageIsInQueue();
		}

		[Test]
		public void While_sending_it_should_perisist_on_complete()
		{
			using (var trx = new TransactionScope())
			{
				SendMessage();

				trx.Complete();
			}


			VerifyMessageIsInQueue();
		}

		[Test]
		public void While_writing_it_should_persist()
		{
			SendMessage();

			VerifyMessageIsInQueue();
		}
	}
}