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
namespace MassTransit.Internal
{
	using System;
	using log4net;
	using Magnum.ObjectExtensions;

	public class MessageHeadersBase :
		ISetMessageHeaders
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (MessageHeadersBase));

		public Uri SourceAddress { get; private set; }
		public Uri DestinationAddress { get; private set; }
		public Uri ResponseAddress { get; private set; }
		public Uri FaultAddress { get; private set; }
		public int RetryCount { get; private set; }
		public string MessageType { get; private set; }

		public void SetSourceAddress(Uri uri)
		{
			SourceAddress = uri;
		}

		public void SetSourceAddress(string uriString)
		{
			SourceAddress = ConvertStringToUri(uriString);
		}

		public void SetDestinationAddress(Uri uri)
		{
			DestinationAddress = uri;
		}

		public void SetDestinationAddress(string uriString)
		{
			DestinationAddress = ConvertStringToUri(uriString);
		}

		public void SetResponseAddress(Uri uri)
		{
			ResponseAddress = uri;
		}

		public void SetResponseAddress(string uriString)
		{
			ResponseAddress = ConvertStringToUri(uriString);
		}

		public void SetFaultAddress(Uri uri)
		{
			FaultAddress = uri;
		}

		public void SetFaultAddress(string uriString)
		{
			FaultAddress = ConvertStringToUri(uriString);
		}

		public void SetRetryCount(int retryCount)
		{
			RetryCount = retryCount;
		}

		public void SetMessageType(Type messageType)
		{
			MessageType = messageType.ToMessageName();
		}

		public void SetMessageType(string messageType)
		{
			MessageType = messageType;
		}

		public virtual void Reset()
		{
			SourceAddress = null;
			DestinationAddress = null;
			ResponseAddress = null;
			FaultAddress = null;
			RetryCount = 0;
			MessageType = null;
		}

		public static Uri ConvertStringToUri(string uriString)
		{
			try
			{
				return uriString.IsNullOrEmpty() ? null : new Uri(uriString);
			}
			catch (UriFormatException ex)
			{
				if (_log.IsWarnEnabled)
					_log.Warn(string.Format("Unable to convert string to Uri: " + uriString), ex);

				return null;
			}
		}
	}
}