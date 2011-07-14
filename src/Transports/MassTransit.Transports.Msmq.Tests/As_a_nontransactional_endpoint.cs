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
namespace MassTransit.Transports.Msmq.Tests
{
	using System;
	using System.Transactions;
	using Magnum.TestFramework;
	using MassTransit.Tests;
	using NUnit.Framework;
	using TestFixtures;
	using TestFramework;

	[TestFixture, Integration]
	public class Writing_to_a_non_transactional_endpoint_within_a_transaction :
		MsmqEndpointOnlyTestFixture
	{
		[Test]
		public void Should_send_the_message_when_a_transaction_is_completed()
		{
			using (TransactionScope transaction = new TransactionScope())
			{
				Endpoint.Send(new DeleteMessage());

				transaction.Complete();
			}

			Endpoint.ShouldContain<DeleteMessage>();
		}

		[Test]
		public void Should_send_the_message_when_a_transaction_is_rolled_back()
		{
			using (TransactionScope transaction = new TransactionScope())
			{
				Endpoint.Send(new DeleteMessage());

				// do not complete the transaction (implicit rollback)
			}

			Endpoint.ShouldContain<DeleteMessage>();
		}
	}

	[TestFixture, Integration]
	public class Writing_to_a_non_transactional_endpoint :
		MsmqEndpointOnlyTestFixture
	{
		[Test]
		public void Should_send_the_message()
		{
			Endpoint.Send(new DeleteMessage());

			Endpoint.ShouldContain<DeleteMessage>();
		}
	}

	[TestFixture, Integration]
	public class Reading_from_a_non_transactional_endpoint_within_a_transaction :
		MsmqEndpointOnlyTestFixture
	{
		protected override void EstablishContext()
		{
			base.EstablishContext();

			Endpoint.Send(new DeleteMessage());
		}

		[Test]
		public void Should_receive_the_message_when_a_transaction_is_completed()
		{
			var future = new Future<DeleteMessage>();

			using (TransactionScope transaction = new TransactionScope())
			{
				Endpoint.Receive(message => m => { future.Complete(m as DeleteMessage); }, TimeSpan.Zero);

				transaction.Complete();
			}

			future.IsCompleted.ShouldBeTrue();
			Endpoint.ShouldNotContain<DeleteMessage>();
		}

		[Test]
		public void Should_receive_the_message_when_a_transaction_is_rolled_back()
		{
			var future = new Future<DeleteMessage>();

			using (TransactionScope transaction = new TransactionScope())
			{
				Endpoint.Receive(message => m => { future.Complete(m as DeleteMessage); }, TimeSpan.Zero);

				// do not complete the transaction (implicit rollback)
			}

			future.IsCompleted.ShouldBeTrue();
			Endpoint.ShouldNotContain<DeleteMessage>();
		}
	}

}