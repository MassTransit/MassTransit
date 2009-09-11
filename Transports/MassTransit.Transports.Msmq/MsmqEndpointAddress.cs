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

	public class MsmqEndpointAddress :
		EndpointAddress,
		IMsmqEndpointAddress
	{
		public MsmqEndpointAddress(Uri uri)
			: base(uri)
		{
			PublicQueuesNotAllowed(uri);

			FormatName = BuildQueueFormatName(uri);

			if (IsLocal)
			{
				Uri = SetUriHostToLocalMachineName(uri);
				LocalName = @".\private$\" + uri.AbsolutePath.Substring(1);
			}
		}

		public string FormatName { get; private set; }

		public string LocalName { get; private set; }

		private static string BuildQueueFormatName(Uri uri)
		{
			string hostName = uri.Host;
			return string.Format(@"FormatName:DIRECT=OS:{0}\private$\{1}", hostName, uri.AbsolutePath.Substring(1));
		}

		private static Uri SetUriHostToLocalMachineName(Uri uri)
		{
			var builder = new UriBuilder(uri.Scheme, LocalMachineName, uri.Port, uri.PathAndQuery);

			return builder.Uri;
		}

		private void PublicQueuesNotAllowed(Uri uri)
		{
			if (!uri.AbsolutePath.Substring(1).Contains("/"))
				return;

			if (uri.AbsolutePath.Substring(1).ToLowerInvariant().Contains("public"))
				throw new NotSupportedException(
					string.Format("Public queues are not supported (please submit a patch): {0}", uri));

			throw new NotSupportedException(
				"MSMQ endpoints do not allow child folders unless it is 'public' (not supported yet, please submit patch). " +
				"Good: 'msmq://machinename/queue_name' or 'msmq://machinename/public/queue_name' - " +
				"Bad: msmq://machinename/round_file/queue_name");
		}
	}
}