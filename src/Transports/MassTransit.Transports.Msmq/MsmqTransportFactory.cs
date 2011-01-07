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

		public ILoopbackTransport BuildLoopback(CreateTransportSettings settings)
		{
			try
			{
				if (settings.Address.IsLocal)
					return NewLocalTransport(settings);

				return NewRemoteTransport(settings);
			}
			catch (Exception ex)
			{
				throw new TransportException(settings.Address.Uri, "Failed to create MSMQ transport", ex);
			}
		}

		public IInboundTransport BuildInbound(CreateTransportSettings settings)
		{
			try
			{
				if (settings.Address.IsLocal)
					return NewLocalInboundTransport(settings);

				return NewRemoteInboundTransport(settings);
			}
			catch (Exception ex)
			{
				throw new TransportException(settings.Address.Uri, "Failed to create MSMQ inbound transport", ex);
			}
		}

		public IOutboundTransport BuildOutbound(CreateTransportSettings settings)
		{
			try
			{
				if (settings.Address.IsLocal)
					return NewLocalOutboundTransport(settings);

				return NewRemoteOutboundTransport(settings);
			}
			catch (Exception ex)
			{
				throw new TransportException(settings.Address.Uri, "Failed to create MSMQ inbound transport", ex);
			}
		}

		public void PurgeExistingMessagesIfRequested(CreateTransportSettings settings)
		{
			if (settings.Address.IsLocal && settings.PurgeExistingMessages)
			{
				MsmqEndpointManagement.Manage(settings.Address, x => x.Purge());
			}
		}


		private IOutboundTransport NewLocalOutboundTransport(CreateTransportSettings settings)
		{
			ValidateLocalTransport(settings);

			PurgeExistingMessagesIfRequested(settings);

			if (settings.Transactional)
				return new TransactionalOutboundMsmqTransport(new MsmqEndpointAddress(settings.Address.Uri));

			return new NonTransactionalOutboundMsmqTransport(new MsmqEndpointAddress(settings.Address.Uri));
		}


		private IInboundTransport NewLocalInboundTransport(CreateTransportSettings settings)
		{
			ValidateLocalTransport(settings);

			PurgeExistingMessagesIfRequested(settings);

			if (settings.Transactional)
				return new TransactionalInboundMsmqTransport(new MsmqEndpointAddress(settings.Address.Uri));

			return new NonTransactionalMsmqTransport(new MsmqEndpointAddress(settings.Address.Uri));
		}


		private ILoopbackTransport NewLocalTransport(CreateTransportSettings settings)
		{
			ValidateLocalTransport(settings);

			PurgeExistingMessagesIfRequested(settings);

			if (settings.Transactional)
				return new TransactionalMsmqTransport(new MsmqEndpointAddress(settings.Address.Uri));

			return new NonTransactionalMsmqTransport(new MsmqEndpointAddress(settings.Address.Uri));
		}

		private static IInboundTransport NewRemoteInboundTransport(CreateTransportSettings settings)
		{
			if (settings.Address.IsTransactional)
				return new TransactionalInboundMsmqTransport(new MsmqEndpointAddress(settings.Address.Uri));

			return new NonTransactionalInboundMsmqTransport(new MsmqEndpointAddress(settings.Address.Uri));
		}

		private static IOutboundTransport NewRemoteOutboundTransport(CreateTransportSettings settings)
		{
			if (settings.Address.IsTransactional)
				return new TransactionalOutboundMsmqTransport(new MsmqEndpointAddress(settings.Address.Uri));

			return new NonTransactionalOutboundMsmqTransport(new MsmqEndpointAddress(settings.Address.Uri));
		}

		private static ILoopbackTransport NewRemoteTransport(CreateTransportSettings settings)
		{
			if (settings.Address.IsTransactional)
				return new TransactionalMsmqTransport(new MsmqEndpointAddress(settings.Address.Uri));

			return new NonTransactionalMsmqTransport(new MsmqEndpointAddress(settings.Address.Uri));
		}

		private static void ValidateLocalTransport(CreateTransportSettings settings)
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
	}
}