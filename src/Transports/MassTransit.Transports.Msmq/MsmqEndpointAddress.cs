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
	using System.Messaging;
	using System.Net;
	using Util;

	public class MsmqEndpointAddress :
		EndpointAddress,
		IMsmqEndpointAddress
	{
		public MsmqEndpointAddress(Uri uri)
			: base(uri)
		{
			PublicQueuesNotAllowed();

			FormatName = BuildQueueFormatName();

			IsTransactional = CheckForTransactionalHint();
		
			if (IsLocal)
			{
				IsTransactional = IsLocalQueueTransactional();

				LocalName = @".\private$\" + Path;

				Uri = SetUriHostToLocalMachineName();
			}
		}

		private bool IsLocalQueueTransactional()
		{
			try
			{
				using(var queue = new MessageQueue(FormatName, QueueAccessMode.PeekAndAdmin))
				{
					return queue.Transactional;
				}
			}
			catch
			{
			}

			return IsTransactional;
		}

		public string FormatName { get; private set; }

		public string LocalName { get; private set; }

		public bool IsTransactional { get; private set; }

		private string BuildQueueFormatName()
		{
			string hostName = Uri.Host;

			if (IsIpAddress(hostName))
				return string.Format(@"FormatName:DIRECT=TCP:{0}\private$\{1}", hostName, Path);

			return string.Format(@"FormatName:DIRECT=OS:{0}\private$\{1}", hostName, Path);
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

		private static bool IsIpAddress(string hostName)
		{
			IPAddress address;
			return IPAddress.TryParse(hostName, out address);
		}

		private bool CheckForTransactionalHint()
		{
			return Uri.Query.GetValueFromQueryString("tx", false);
		}

		private Uri SetUriHostToLocalMachineName()
		{
			string query = "?tx=" + IsTransactional.ToString().ToLowerInvariant();

			var builder = new UriBuilder(Uri.Scheme, LocalMachineName, Uri.Port, Uri.AbsolutePath, query);

			return builder.Uri;
		}
	}
}