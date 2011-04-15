// Copyright 2007-2011 The Apache Software Foundation.
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
			_serializer = new CustomXmlMessageSerializer();
			_transport = _factory.BuildOutbound(new CreateTransportSettings(new EndpointAddress(Address)));
		}

		[TearDown]
		public void TearDown()
		{
			_transport.Dispose();
		}

		private IOutboundTransport _transport;
		private readonly ITransportFactory _factory;
		private IMessageSerializer _serializer;

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

		public void VerifyMessageIsInQueue(ITransport ep)
		{
			IInboundTransport tr = _factory.BuildInbound(new CreateTransportSettings(new EndpointAddress(AddressToCheck)));
			tr.ShouldContain<DeleteMessage>(_serializer);
		}


		[Test]
		public void While_sending_it_should_perisist_even_on_rollback()
		{
			using (var trx = new TransactionScope())
			{
				_transport.Send(context => { _serializer.Serialize(context.Body, new DeleteMessage()); });

				//no complete
			}

			VerifyMessageIsInQueue(_transport);
		}

		[Test]
		public void While_sending_it_should_perisist_on_complete()
		{
			using (var trx = new TransactionScope())
			{
				_transport.Send(context => { _serializer.Serialize(context.Body, new DeleteMessage()); });

				trx.Complete();
			}


			VerifyMessageIsInQueue(_transport);
		}

		[Test]
		public void While_writing_it_should_persist()
		{
			_transport.Send(context => { _serializer.Serialize(context.Body, new DeleteMessage()); });

			VerifyMessageIsInQueue(_transport);
		}
	}
}