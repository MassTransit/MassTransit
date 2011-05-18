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
	using Magnum;
	using Magnum.Extensions;
	using RabbitMQ.Client;
	using Util;

	public class RabbitMqEndpointAddress :
		IRabbitMqEndpointAddress
	{
		const string FormatErrorMsg =
			"The path can be empty, or a sequence of these characters: letters, digits, hyphen, underscore, period, or colon.";

		static readonly string LocalMachineName = Environment.MachineName.ToLowerInvariant();

		static readonly Regex _regex = new Regex(@"^[A-Za-z0-9\-_\.:]+$");
		readonly ConnectionFactory _connectionFactory;

		readonly bool _isTransactional;
		readonly string _name;
		readonly Uri _uri;
		Func<bool> _isLocal;


		public RabbitMqEndpointAddress(Uri uri, ConnectionFactory connectionFactory, string name)
		{
			_uri = uri;
			_connectionFactory = connectionFactory;
			_name = name;

			_isTransactional = uri.Query.GetValueFromQueryString("tx", false);
			_isLocal = () => DetermineIfEndpointIsLocal(_uri);
		}

		public ConnectionFactory ConnectionFactory
		{
			get { return _connectionFactory; }
		}

		public string Name
		{
			get { return _name; }
		}

		public IRabbitMqEndpointAddress ForQueue(string name)
		{
			string uri = _uri.ToString();
			uri = uri.Remove(uri.Length - _name.Length);

			return new RabbitMqEndpointAddress(new Uri(uri).AppendPath(name), _connectionFactory, name);
		}

		public Uri Uri
		{
			get { return _uri; }
		}

		public bool IsLocal
		{
			get { return _isLocal(); }
		}

		public bool IsTransactional
		{
			get { return _isTransactional; }
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

		public static RabbitMqEndpointAddress Parse(string address)
		{
			return Parse(new Uri(address));
		}

		public static RabbitMqEndpointAddress Parse(Uri address)
		{
			Guard.AgainstNull(address, "address");

			if (string.Compare("rabbitmq", address.Scheme, true) != 0)
				throw new RabbitMqAddressException("The invalid scheme was specified: " + address.Scheme ?? "(null)");

			var connectionFactory = new ConnectionFactory
				{
					HostName = address.Host,
				};

			if (address.IsDefaultPort)
				connectionFactory.Port = 5672;
			else if (!address.IsDefaultPort)
				connectionFactory.Port = address.Port;

			if (!address.UserInfo.IsEmpty())
			{
				if (address.UserInfo.Contains(":"))
				{
					string[] parts = address.UserInfo.Split(':');
					connectionFactory.UserName = parts[0];
					connectionFactory.Password = parts[1];
				}
				else
				{
					connectionFactory.UserName = address.UserInfo;
				}
			}

			string name = address.AbsolutePath.Substring(1);
			string[] pathSegments = name.Split('/');
			if (pathSegments.Length == 2)
			{
				connectionFactory.VirtualHost = pathSegments[0];
				name = pathSegments[1];
			}

			VerifyQueueOrExchangeNameIsLegal(name);

			return new RabbitMqEndpointAddress(address, connectionFactory, name);
		}

		static void VerifyQueueOrExchangeNameIsLegal(string path)
		{
			Match match = _regex.Match(path);

			if (!match.Success)
				throw new RabbitMqAddressException(FormatErrorMsg);
		}
	}
}