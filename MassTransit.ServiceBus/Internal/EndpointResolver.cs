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
	using System.Globalization;
	using System.Reflection;
	using System.Text;
	using System.Threading;
	using Exceptions;
	using log4net;
	using Magnum.Common.Threading;

    public class EndpointResolver :
		IEndpointResolver
	{
		private static readonly Dictionary<Uri, IEndpoint> _cache = new Dictionary<Uri, IEndpoint>();
		private static readonly ILog _log = LogManager.GetLogger(typeof (EndpointResolver));
		private static readonly Dictionary<string, Type> _schemes = new Dictionary<string, Type>();
	    private static readonly IEndpoint _null = new NullEndpoint();
        private static readonly UpgradeableLock _lockContext = new UpgradeableLock();

		public static string AddTransport(Type transportType)
		{
            using (var token = _lockContext.EnterUpgradableRead())
            {
                // get the scheme for each endpoint and add it to the resolver
                const BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField | BindingFlags.NonPublic;

                PropertyInfo property = transportType.GetProperty("Scheme", bindingFlags, null, typeof (string), new Type[0], null);

                string scheme = property.GetValue(null, bindingFlags, null, null, CultureInfo.InvariantCulture) as string;
                if (string.IsNullOrEmpty(scheme))
                    throw new ConventionException("No endpoint scheme defined for transport");

                if (_schemes.ContainsKey(scheme))
                    return scheme;

                using(token.Upgrade())
                {
                    if (_schemes.ContainsKey(scheme))
                        return scheme;

                    _log.InfoFormat("Registering transport '{0}' to schema '{1}'", transportType.Name, scheme);

                    _schemes.Add(scheme, transportType);

                    return scheme;
                }
            }
		}

		public static ConstructorInfo GetConstructor(Type type)
        {
            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
            const CallingConventions conventions = CallingConventions.Standard | CallingConventions.HasThis;
            Type[] parameterTypes = new[] { typeof(Uri) };

            return type.GetConstructor(bindingFlags, null, conventions, parameterTypes, null);
        }

		public IEndpoint Resolve(Uri uri)
		{
			GuardAgainstZeroTransports();

		    using(var token = _lockContext.EnterUpgradableRead())
		    {
		        if (_cache.ContainsKey(uri))
					return _cache[uri];

                using(token.Upgrade())
                {
                    if (_cache.ContainsKey(uri))
                        return _cache[uri];

                    if (_schemes.ContainsKey(uri.Scheme))
                    {
                        IEndpoint endpoint = GetEndpoint(uri);
                        
                        _cache.Add(uri, endpoint);

                        return endpoint;
                    }
                }
		    }

			string message = BuildHelpfulErrorMessage(uri);
			_log.Error(message);
			throw new EndpointException(new NullEndpoint(), message);
		}

        private IEndpoint GetEndpoint(Uri uri)
        {
            try
            {
                object obj = Activator.CreateInstance(_schemes[uri.Scheme], uri);
                if (obj == null)
                    throw new ArgumentException("Unable to create endpoint from uri: " + uri);

                var endpoint = obj as IEndpoint;
                if (endpoint == null)
                    throw new ArgumentException("The type was not converted to an endpoint: " + _schemes[uri.Scheme]);

                return endpoint;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private static string BuildHelpfulErrorMessage(Uri uri)
	    {
	        StringBuilder sb = new StringBuilder();
	        sb.AppendLine("Unable to resolve Uri " + uri + " to an endpoint");
            sb.AppendFormat("We could not match your uri's schema '{0}' with a registered transport. Available schemas are: {1}", uri.Scheme, Environment.NewLine);
	        foreach (KeyValuePair<string, Type> pair in _schemes)
	        {
	            sb.AppendLine(pair.Key);
	        }
	        return sb.ToString();
	    }

	    private static void GuardAgainstZeroTransports()
	    {
            if (_schemes.Count == 0)
            {
                const string message = "No transports have been registered. Please use EndpointResolver.AddTransport";
                EndpointException exp = new EndpointException(new NullEndpoint(), message);
                _log.Error(message, exp);
                throw exp;
            }
	    }

	    public static IEndpoint Null
	    {
	        get { return _null; }
	    }

        public static void EnsureThatTransportsExist(IList<Uri> uris)
        {
            foreach (var uri in uris)
                EnsureTransports(uri);
        }

	    public static void EnsureTransports(Uri uri)
	    {
            if(!_schemes.ContainsKey(uri.Scheme))
                throw new EndpointException(new NullEndpoint(), "No transport for " + uri);
	    }
	}
}