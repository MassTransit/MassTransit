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
namespace MassTransit.Transports.Msmq.Tests
{
	using System;
	using NUnit.Framework;
	using TestFramework.Transports;

	[TestFixture, Category("Integration")]
	public class MsmqNonTransactionalSendingContract :
		NonTransactionalTransportSendingContract<MsmqTransportFactory>
	{
		public MsmqNonTransactionalSendingContract()
			: base(new Uri("msmq://localhost/mt_client"), new MsmqTransportFactory())
		{
			EndpointCacheFactory.ConfigureDefaultSettings(x =>
				{
					x.SetCreateMissingQueues(true);
					x.SetCreateTransactionalQueues(false);
					x.SetPurgeOnStartup(true);
				});
		}
	}
}