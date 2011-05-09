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
namespace MassTransit.Transports.RabbitMq
{
	using System;
	using System.Text.RegularExpressions;
	using System.Threading;
	using Magnum.Extensions;
	using Util;

	public class RabbitMqAddress :
		IEndpointAddress
	{
		const int DefaultPort = 5432;

		const string FormatErrorMsg =
			"The path can be empty, or a sequence of these characters: letters, digits, hyphen, underscore, period, or colon.";

		static readonly string LocalMachineName = Environment.MachineName.ToLowerInvariant();

		static readonly Regex _regex = new Regex(@"^[A-Za-z0-9\-_\.:]+$");
		readonly bool _isTransactional;
		readonly Uri _rawUri;
		readonly Uri _rebuiltUri;
		Func<bool> _isLocal;


		public RabbitMqAddress(Uri uri, Uri rebuiltUri, string username, string password, string vhost, string path,
		                       string args)
		{
			_rawUri = uri;
			_rebuiltUri = rebuiltUri;

			Username = username;
			Password = password;
			Host = rebuiltUri.Host;
			Path = path;
			VHost = vhost;
			Port = rebuiltUri.Port;
			_isTransactional = args.GetValueFromQueryString("tx", false);
			_isLocal = () => DetermineIfEndpointIsLocal(_rawUri);
		}

		public int Port { get; private set; }
		public string VHost { get; private set; }
		public string Host { get; private set; }
		public string Username { get; private set; }
		public string Password { get; private set; }

		public Uri RawUri
		{
			get { return _rawUri; }
		}

		public Uri RebuiltUri
		{
			get { return _rebuiltUri; }
		}

		public Uri Uri
		{
			get { return _rebuiltUri; }
		}

		public bool IsLocal
		{
			get { return _isLocal(); }
		}

		public string Path { get; private set; }

		public bool IsTransactional
		{
			get { return _isTransactional; }
		}

		public Uri GetConnectionUri()
		{
			return new UriBuilder("", _rawUri.Host, _rawUri.Port, "").Uri;
		}

		public bool Equals(RabbitMqAddress other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other._rebuiltUri, _rebuiltUri);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (RabbitMqAddress)) return false;
			return Equals((RabbitMqAddress) obj);
		}

		public override int GetHashCode()
		{
			return (_rebuiltUri != null ? _rebuiltUri.GetHashCode() : 0);
		}

		bool DetermineIfEndpointIsLocal(Uri uri)
		{
			string hostName = uri.Host;
			bool local = string.Compare(hostName, ".") == 0 ||
			             string.Compare(hostName, "localhost", true) == 0 ||
			             string.Compare(uri.Host, LocalMachineName, true) == 0;

			Interlocked.Exchange(ref _isLocal, () => local);

			return local;
		}

		int ParsePort()
		{
			return _rawUri.Port == -1 ? DefaultPort : _rawUri.Port;
		}

		string ParseVHost(Uri uri)
		{
			string[] bits = uri.LocalPath.Split('/');

			if (bits.Length == 3)
				return @"/";
			else
				return bits[1];
		}

		string ParseQueue(Uri uri)
		{
			string[] bits = uri.LocalPath.Split('/');
			if (bits.Length == 3)
				return bits[2];
			else
				return bits[3];
		}

		public static RabbitMqAddress Parse(string address)
		{
			return Parse(new Uri(address));
		}

		public static RabbitMqAddress Parse(Uri address)
		{
			int port = address.Port == -1 ? DefaultPort : address.Port;
			string user = "guest";
			string password = "guest";
			if (!address.UserInfo.IsEmpty())
			{
				string[] parts = address.UserInfo.Split(':');
				user = parts[0];
				password = parts[1];
			}
			string[] pathbits = address.LocalPath.Split('/');
			string vhost = "/";
			string path = address.LocalPath.Substring(1); //removes first '/'
			if (pathbits.Length == 3)
			{
				vhost = pathbits[1];
				path = pathbits[2];
			}

			VerifyRabbitPath(path);

			var ub = new UriBuilder(address.Scheme, address.Host, port, address.LocalPath);
			ub.UserName = user;
			ub.Password = password;

			return new RabbitMqAddress(address, ub.Uri, user, password, vhost, path, address.Query);
		}

		static void VerifyRabbitPath(string path)
		{
			Match m = _regex.Match(path);

			if (!m.Success)
				throw new RabbitMqAddressException(FormatErrorMsg);
		}
	}
}