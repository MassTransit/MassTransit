// Copyright 2007-2008 The Apache Software Foundation.
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
	using System.Diagnostics;
	using System.Messaging;
	using System.Transactions;
	using log4net;
	using Magnum.Actors;
	using Magnum.Actors.CommandQueues;
	using Magnum.DateTimeExtensions;

	public class StoreAndForwardMsmqTransport :
		AbstractMsmqTransport
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (StoreAndForwardMsmqTransport));
		private readonly CommandQueue _commandQueue = new ThreadPoolCommandQueue();
		private readonly TimeSpan _forwardTimeout = 1.Seconds();
		private bool _disposed;
		private IMsmqTransport _remoteTransport;

		public StoreAndForwardMsmqTransport(IMsmqEndpointAddress address, IMsmqTransport remoteTransport)
			: base(address)
		{
			_remoteTransport = remoteTransport;

			_commandQueue.Enqueue(ForwardMessage);
		}

		protected override void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_commandQueue.Disable();

				_remoteTransport.Dispose();
				_remoteTransport = null;
			}
			_disposed = true;

			base.Dispose(disposing);
		}

		private void ForwardMessage()
		{
			try
			{
				using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew))
				{
					Receive(TransferMessageFromLocalToRemote, _forwardTimeout);

					transaction.Complete();
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.Message);

				_log.Error("Unable to forward message", ex);
			}

			_commandQueue.Enqueue(ForwardMessage);
		}

		private Action<Message> TransferMessageFromLocalToRemote(Message local)
		{
			Trace.WriteLine("Forwarding message: " + local.Id);

			return outbound => _remoteTransport.Send(remote => remote.BodyStream = outbound.BodyStream);
		}
	}
}