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
	using System.Messaging;
	using Util;

    public class MsmqEndpointAddress :
		EndpointAddress,
		IMsmqEndpointAddress
	{
        public MsmqEndpointAddress(Uri uri)
            : this(uri, false, true)
		{
		}

	    public MsmqEndpointAddress(Uri uri, bool defaultTransactional, bool defaultRecoverable)
			: base(uri)
		{
			PublicQueuesNotAllowed();

			InboundFormatName = uri.GetInboundFormatName();

		    InboundUri = uri.GetInboundUri();

			OutboundFormatName = uri.GetOutboundFormatName();

			IsTransactional = CheckForTransactionalHint(uri, defaultTransactional);

	        IsRecoverable = CheckForRecoverableHint(uri, defaultRecoverable);

			MulticastAddress = uri.GetMulticastAddress();
			if(MulticastAddress != null)
			{
				IsTransactional = false;
				LocalName = uri.GetLocalName();
			}
			else if (IsLocal)
			{
				IsTransactional = IsLocalQueueTransactional();

				LocalName = uri.GetLocalName();

				Uri = SetUriHostToLocalMachineName();
			}
		}

		public string InboundFormatName { get; private set; }

	    public Uri InboundUri { get; private set; }

	    public string OutboundFormatName { get; private set; }

	    public bool IsRecoverable { get; private set; }

	    public string LocalName { get; private set; }

		public string MulticastAddress { get; private set; }

		private bool IsLocalQueueTransactional()
		{
			try
			{
				using (var queue = new MessageQueue(InboundFormatName, QueueAccessMode.PeekAndAdmin))
				{
					return queue.Transactional;
				}
			}
			catch
			{
			}

			return IsTransactional;
		}


		private void PublicQueuesNotAllowed()
		{
			if (!Path.Contains("/"))
				return;

			if (Path.ToLowerInvariant().Contains("public"))
				throw new NotSupportedException(
					string.Format("Public queues are not supported (please submit a patch): {0}", Uri));

			throw new NotSupportedException(
				"MSMQ endpoints do not allow child folders unless it is 'public' (not supported yet, please submit patch). " +
				"Good: 'msmq://machinename/queue_name' or 'msmq://machinename/public/queue_name' - " +
				"Bad: msmq://machinename/round_file/queue_name");
		}

		protected override bool DetermineIfEndpointIsLocal(Uri uri)
		{
			if (uri.Scheme.ToLowerInvariant() == "msmq-pgm")
				return true;

			return base.DetermineIfEndpointIsLocal(uri);
		}

		private Uri SetUriHostToLocalMachineName()
		{
		    string query = String.Format("?tx={0}&recoverable={1}",
                IsTransactional.ToString().ToLowerInvariant(), 
                IsRecoverable.ToString().ToLowerInvariant());

			var builder = new UriBuilder(Uri.Scheme, LocalMachineName, Uri.Port, Uri.AbsolutePath, query);
            
			return builder.Uri;
		}

        protected static bool CheckForRecoverableHint(Uri uri, bool defaultRecoverable)
        {
            return uri.Query.GetValueFromQueryString("recoverable", defaultRecoverable);
        }
	}
}