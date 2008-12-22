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

	/// <summary>
	/// Deals with the various version of queue addresses required when dealing with MSMQ
	/// </summary>
	public class QueueAddress
	{
		private const string _localhost = "localhost";

		static QueueAddress()
		{
			LocalMachineName = Environment.MachineName.ToLowerInvariant();
		}

		public QueueAddress(Uri uri)
		{
			PublicQueuesNotAllowed(uri);

			IsLocal = IsUriHostLocal(uri);

			ActualUri = IsLocal ? SetUriHostToLocalMachineName(uri) : uri;

			FormatName = BuildQueueFormatName(uri);
		}

		public QueueAddress(MessageQueue queue)
		{
			string path = queue.Path;
			const string prefix = "FormatName:DIRECT=OS:";

			if (path.Length > prefix.Length && path.Substring(0, prefix.Length).ToUpperInvariant() == prefix.ToUpperInvariant())
				path = path.Substring(prefix.Length);

			string[] parts = path.Split('\\');

			if (parts.Length != 3)
				throw new ArgumentException("Invalid Queue Path Specified");

			//Validate parts[1] = private$
			if (string.Compare(parts[1], "private$", true) != 0)
				throw new ArgumentException("Invalid Queue Path Specified");

			string localhost = Environment.MachineName.ToLowerInvariant();
			if (parts[0] == "." || string.Compare("localhost", parts[0], true) == 0)
			{
				parts[0] = localhost;
				IsLocal = true;
			}
			else
			{
				parts[0] = parts[0].ToLowerInvariant();
				IsLocal = string.Compare(localhost, parts[0], true) == 0;
			}

			FormatName = string.Format("{0}{1}\\{2}\\{3}", prefix, parts[0], parts[1], parts[2]);

			ActualUri = new Uri(string.Format("msmq://{0}/{1}", parts[0], parts[2]));
		}

		public QueueAddress(string address)
			: this(new Uri(address))
		{
		}

		/// <summary>
		/// The local machine name used when publishing local queues
		/// </summary>
		public static string LocalMachineName { get; private set; }

		/// <summary>
		/// True if the queue is local to this machine
		/// </summary>
		public bool IsLocal { get; private set; }

		/// <summary>
		/// The Uri specified by the caller, unmodified
		/// </summary>
		public Uri ActualUri { get; private set; }

		/// <summary>
		/// The format name used to talk to MSMQ
		/// </summary>
		public string FormatName { get; private set; }

		private static string BuildQueueFormatName(Uri uri)
		{
			string hostName = uri.Host;
			return string.Format(@"FormatName:DIRECT=OS:{0}\private$\{1}", hostName, uri.AbsolutePath.Substring(1));
		}

		private static Uri SetUriHostToLocalMachineName(Uri uri)
		{
			UriBuilder builder = new UriBuilder(uri.Scheme, LocalMachineName, uri.Port, uri.PathAndQuery);

			return builder.Uri;
		}

		private static bool IsUriHostLocal(Uri uri)
		{
			string hostName = uri.Host;
			return string.Compare(hostName, ".") == 0 ||
				string.Compare(hostName, _localhost, true) == 0 ||
					string.Compare(uri.Host, LocalMachineName, true) == 0;
		}

		private static void PublicQueuesNotAllowed(Uri uri)
		{
			if (!uri.AbsolutePath.Substring(1).Contains("/"))
				return;

			if (uri.AbsolutePath.Substring(1).ToLowerInvariant().Contains("public"))
				throw new NotSupportedException(string.Format("Public queues are not supported (please submit a patch): {0}", uri));

			throw new UriParseException(
				"MSMQ endpoints do not allow child folders unless it is 'public' (not supported yet, please submit patch). " +
					"Good: 'msmq://machinename/queue_name' or 'msmq://machinename/public/queue_name' - " +
						"Bad: msmq://machinename/round_file/queue_name");
		}
	}
}