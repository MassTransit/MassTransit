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
	using System.Reflection;
	using System.Text;
	using Exceptions;
	using log4net;

	public class EndpointResolver :
		IEndpointResolver
	{
		private static readonly Dictionary<Uri, IEndpoint> _cache = new Dictionary<Uri, IEndpoint>();
		private static readonly ILog _log = LogManager.GetLogger(typeof (EndpointResolver));
		private static readonly Dictionary<string, Type> _schemes = new Dictionary<string, Type>();

		public static void AddTransport(string scheme, Type t)
		{
			lock (_schemes)
			{
				if (_schemes.ContainsKey(scheme))
					return;

                _log.InfoFormat("Registering transport '{0}' to schema '{1}'", t.Name, scheme);
				_schemes.Add(scheme, t);
			}
				
		}

        public static ConstructorInfo GetConstructor(Type type)
        {
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
            CallingConventions conventions = CallingConventions.Standard | CallingConventions.HasThis;
            Type[] parameterTypes = new Type[] { typeof(Uri) };

            return type.GetConstructor(bindingFlags, null, conventions, parameterTypes, null);
        }

		public IEndpoint Resolve(Uri uri)
		{
            GuardAgainstZeroTransports();

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

		    string message = BuildHelpfulErrorMessage(uri);
            _log.Error(message);
			throw new EndpointException(new NullEndpoint(), message);
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
                string message = "No transports have been registered. Please use EndpointResolver.AddTransport";
                EndpointException exp = new EndpointException(new NullEndpoint(), message);
                _log.Error(message, exp);
                throw exp;
            }
	    }

	    public class NullEndpoint :
            IEndpoint
        {
            #region Implementation of IDisposable

            public void Dispose()
            {
                //ignore
            }

            #endregion

            #region Implementation of IEndpoint

            public Uri Uri
            {
                get { return new Uri("none://nosuchuri"); }
            }

            public void Send<T>(T message) where T : class
            {
                //do nothing
            }

            public void Send<T>(T message, TimeSpan timeToLive) where T : class
            {
                //do nothing
            }

            public object Receive()
            {
                return new object();
            }

            public object Receive(TimeSpan timeout)
            {
                return new object();
            }

            public object Receive(Predicate<object> accept)
            {
                return new object();
            }

            public object Receive(TimeSpan timeout, Predicate<object> accept)
            {
                return new object();
            }

            public T Receive<T>() where T : class
            {
                throw new System.NotImplementedException();
            }

            public T Receive<T>(TimeSpan timeout) where T : class
            {
                throw new System.NotImplementedException();
            }

            public T Receive<T>(Predicate<T> accept) where T : class
            {
                throw new System.NotImplementedException();
            }

            public T Receive<T>(TimeSpan timeout, Predicate<T> accept) where T : class
            {
                throw new System.NotImplementedException();
            }

            #endregion
        }

	}
}