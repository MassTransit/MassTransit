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
namespace MassTransit.Tests.Transports
{
	using System;
	using System.Transactions;
	using MassTransit.Transports;
	using NUnit.Framework;

	public abstract class TransactionalEndpointContract<TTransportFactory>
		where TTransportFactory : class, ITransportFactory, new()
	{
		IEndpointCache _endpointCache;
		IEndpoint _endpoint;
		public Uri Address { get; set; }
		public Action<Uri> VerifyMessageIsInQueue { get; set; }
		public Action<Uri> VerifyMessageIsNotInQueue { get; set; }

		[SetUp]
		public void SetUp()
		{
			_endpointCache = EndpointCacheFactory.New(c => { c.AddTransportFactory<TTransportFactory>(); });
			_endpoint = _endpointCache.GetEndpoint(Address);
		}

		[TearDown]
		public void TearDown()
		{
			_endpoint.Dispose();
			_endpoint = null;
		}


		[Test]
		public void While_writing_it_should_perisist_on_complete()
		{
			using (var trx = new TransactionScope())
			{
				_endpoint.Send(new DeleteMessage());
				trx.Complete();
			}


			VerifyMessageIsInQueue(Address);
		}

		[Test]
		public void While_writing_it_should_not_perisist_on_rollback()
		{
			using (new TransactionScope())
			{
				_endpoint.Send(new DeleteMessage());
				//no complete
			}

			VerifyMessageIsNotInQueue(Address);
		}


		//outside transaction
		[Test]
		public void It_should_auto_enlist_a_transaction_and_persist()
		{
			_endpoint.Send(new DeleteMessage());
			VerifyMessageIsNotInQueue(Address);
		}
	}
}