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
namespace MassTransit.Transports.Msmq
{
	using System;
	using Exceptions;

	public class MsmqTransportFactory :
		ITransportFactory
	{
		public string Scheme
		{
			get { return "msmq"; }
		}

		public IDuplexTransport BuildLoopback(ITransportSettings settings)
		{
			try
			{
				return new Transport(BuildInbound(settings), BuildOutbound(settings));
			}
			catch (Exception ex)
			{
				throw new TransportException(settings.Address.Uri, "Failed to create MSMQ transport", ex);
			}
		}

		public IInboundTransport BuildInbound(ITransportSettings settings)
		{
			try
			{
				var msmqEndpointAddress = new MsmqEndpointAddress(settings.Address.Uri);
				var msmqSettings = new TransportSettings(msmqEndpointAddress, settings)
					{
						Transactional = msmqEndpointAddress.IsTransactional
					};

				if (msmqSettings.MsmqAddress().IsLocal)
				{
					ValidateLocalTransport(msmqSettings);

					PurgeExistingMessagesIfRequested(msmqSettings);
				}

				if (msmqSettings.Transactional)
					return new TransactionalInboundMsmqTransport(msmqSettings.MsmqAddress(),
						msmqSettings.TransactionTimeout);

				return new NonTransactionalInboundMsmqTransport(msmqSettings.MsmqAddress());
			}
			catch (Exception ex)
			{
				throw new TransportException(settings.Address.Uri, "Failed to create MSMQ inbound transport", ex);
			}
		}

		public IOutboundTransport BuildOutbound(ITransportSettings settings)
		{
			try
			{
				var msmqEndpointAddress = new MsmqEndpointAddress(settings.Address.Uri);
				var msmqSettings = new TransportSettings(msmqEndpointAddress, settings)
				{
					Transactional = msmqEndpointAddress.IsTransactional
				};

				if (msmqSettings.MsmqAddress().IsLocal)
				{
					ValidateLocalTransport(msmqSettings);
				}

				if (msmqSettings.Transactional)
					return new TransactionalOutboundMsmqTransport(msmqSettings.MsmqAddress());

				return new NonTransactionalOutboundMsmqTransport(msmqSettings.MsmqAddress());
			}
			catch (Exception ex)
			{
				throw new TransportException(settings.Address.Uri, "Failed to create MSMQ outbound transport", ex);
			}
		}

		public IOutboundTransport BuildError(ITransportSettings settings)
		{
			return BuildOutbound(settings);
		}

		static void PurgeExistingMessagesIfRequested(ITransportSettings settings)
		{
			if (settings.Address.IsLocal && settings.PurgeExistingMessages)
			{
				MsmqEndpointManagement.Manage(settings.Address, x => x.Purge());
			}
		}

		static void ValidateLocalTransport(ITransportSettings settings)
		{
			MsmqEndpointManagement.Manage(settings.Address, q =>
				{
					if (!q.Exists)
					{
						if (!settings.CreateIfMissing)
							throw new TransportException(settings.Address.Uri,
								"The transport does not exist and automatic creation is not enabled");

						q.Create(settings.Transactional || settings.Address.IsTransactional);
					}

					if (settings.RequireTransactional)
					{
						if (!q.IsTransactional && (settings.Transactional || settings.Address.IsTransactional))
							throw new TransportException(settings.Address.Uri,
								"The transport is non-transactional but a transactional transport was requested");
					}
				});
		}

		public void Dispose()
		{
		}
	}
}