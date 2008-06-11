/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.
namespace MassTransit.ServiceBus.Internal
{
	using System;
	using System.Collections.Generic;
	using log4net;

	public class EndpointResolver :
		IEndpointResolver
	{
		private static readonly Dictionary<Uri, IEndpoint> _cache = new Dictionary<Uri, IEndpoint>();
		private static readonly ILog _log = LogManager.GetLogger(typeof (EndpointResolver));
		private static readonly Dictionary<string, Type> _schemes = new Dictionary<string, Type>();

		public IEndpoint Resolve(Uri uri)
		{
			lock (_cache)
			{
				if (_cache.ContainsKey(uri))
					return _cache[uri];

				lock (_schemes)
				{
					if (_cache.ContainsKey(uri))
						return _cache[uri];

					if (_schemes.ContainsKey(uri.Scheme))
					{
						object obj = Activator.CreateInstance(_schemes[uri.Scheme], uri);
						if (obj == null)
							throw new ArgumentException("Unable to create endpoint from uri: " + uri);

						IEndpoint endpoint = obj as IEndpoint;
						if (endpoint == null)
							throw new ArgumentException("The type was not converted to an endpoint: " + _schemes[uri.Scheme]);

						_cache.Add(uri, endpoint);

						return endpoint;
					}
				}
			}

			throw new ArgumentException("Unable to resolve Uri " + uri + " to an endpoint");
		}

		public static void AddTransport(string scheme, Type t)
		{
			lock (_schemes)
				_schemes.Add(scheme, t);
		}
	}
}