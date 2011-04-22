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
namespace MassTransit
{
	using System;
	using System.Threading;
	using Magnum;
	using Util;

	public class EndpointAddress :
		IEndpointAddress
	{
		protected static readonly string LocalMachineName = Environment.MachineName.ToLowerInvariant();
		private static readonly IEndpointAddress _nullEndpointAddress = new EndpointAddress(new Uri("null://nul/nul"));
		private bool _isTransactional;
		private Uri _uri;

		private Func<bool> _isLocal;

		public EndpointAddress(Uri uri)
		{
			Guard.AgainstNull(uri, "uri");

			_uri = uri;

			_isLocal = () => DetermineIfEndpointIsLocal(uri);

			_isTransactional = CheckForTransactionalHint(uri);
		}

		public EndpointAddress(string uriString)
		{
			Guard.AgainstEmpty(uriString, "uriString");

			try
			{
				_uri = new Uri(uriString);
			}
			catch (UriFormatException ex)
			{
				throw new ArgumentException("The URI is invalid: " + uriString, ex);
			}

			_isLocal = () => DetermineIfEndpointIsLocal(_uri);

			_isTransactional = CheckForTransactionalHint(_uri);
		}

		public static IEndpointAddress Null
		{
			get { return _nullEndpointAddress; }
		}

		public Uri Uri
		{
			get { return _uri; }
			protected set { _uri = value; }
		}

		public bool IsLocal
		{
			get { return _isLocal(); }
		}

		public string Path
		{
			get { return Uri.AbsolutePath.Substring(1); }
		}

		public bool IsTransactional
		{
			get { return _isTransactional; }
			protected set { _isTransactional = value; }
		}

		public override string ToString()
		{
			return _uri.ToString();
		}

		protected static bool CheckForTransactionalHint(Uri uri)
		{
			return uri.Query.GetValueFromQueryString("tx", false);
		}

		private bool DetermineIfEndpointIsLocal(Uri uri)
		{
			string hostName = uri.Host;
			bool local = string.Compare(hostName, ".") == 0 ||
			             string.Compare(hostName, "localhost", true) == 0 ||
			             string.Compare(uri.Host, LocalMachineName, true) == 0;

			Interlocked.Exchange(ref _isLocal, () => local);

			return local;
		}
	}
}