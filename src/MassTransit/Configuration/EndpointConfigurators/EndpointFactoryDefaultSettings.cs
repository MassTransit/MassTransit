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
namespace MassTransit.EndpointConfigurators
{
	using System;
	using System.Transactions;
	using Magnum.Extensions;
	using Serialization;
	using Transports;

	public class EndpointFactoryDefaultSettings :
		IEndpointFactoryDefaultSettings
	{
		public EndpointFactoryDefaultSettings()
		{
			CreateMissingQueues = true;
			PurgeOnStartup = false;
			RequireTransactional = false;
			Serializer = new XmlMessageSerializer();
			TransactionTimeout = 30.Seconds();
			IsolationLevel = IsolationLevel.Serializable;
		}

		public EndpointFactoryDefaultSettings(IEndpointFactoryDefaultSettings defaults)
		{
			CreateMissingQueues = defaults.CreateMissingQueues;
			PurgeOnStartup = defaults.PurgeOnStartup;
			RequireTransactional = defaults.RequireTransactional;
			Serializer = defaults.Serializer;
			TransactionTimeout = defaults.TransactionTimeout;
			CreateTransactionalQueues = defaults.CreateTransactionalQueues;
		}

		public TimeSpan TransactionTimeout { get; set; }

		public IsolationLevel IsolationLevel { get; set; }

		public IMessageSerializer Serializer { get; set; }

		public bool CreateMissingQueues { get; set; }

		public bool RequireTransactional { get; set; }

		public bool PurgeOnStartup { get; set; }

		public bool CreateTransactionalQueues { get; set; }

		public EndpointSettings CreateEndpointSettings(Uri uri)
		{
			var settings = new EndpointSettings(uri)
				{
					Serializer = Serializer,
					CreateIfMissing = CreateMissingQueues,
					TransactionTimeout = TransactionTimeout,
					PurgeExistingMessages = PurgeOnStartup,
					RequireTransactional = RequireTransactional,
				};

			return settings;
		}
	}
}