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
	using Util;

	public class EndpointResolver
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (EndpointResolver));
		private readonly Dictionary<Uri, IEndpoint> _cache = new Dictionary<Uri, IEndpoint>();

		private readonly List<Type> _endpointTypes = new List<Type>();
		private readonly Dictionary<string, Type> _schemeTypes = new Dictionary<string, Type>();
		private Assembly[] _assemblies;
		private bool _initialized = false;

		public void Initialize()
		{
			lock (_endpointTypes)
			{
				_endpointTypes.Clear();

				Type endpointType = typeof (IEndpoint);

				_assemblies = AppDomain.CurrentDomain.GetAssemblies();

                foreach (Assembly assembly in _assemblies)
                {
                    try
                    {
                        Type[] types = assembly.GetTypes();
                        foreach (Type type in types)
                        {
                            if (type.IsAbstract)
                                continue;

                            if (endpointType.IsAssignableFrom(type))
                            {
                                _endpointTypes.Add(type);
                            }
                        }
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        StringBuilder sb = new StringBuilder();
                        int i = 0;
                        sb.AppendLine("EndpointResolver Error");
                        sb.AppendLine(assembly.FullName);
                        foreach (Exception exception in ex.LoaderExceptions)
                        {
                            sb.AppendFormat("{0}:{1} {2}", i, exception.Message, Environment.NewLine);
                            i++;
                        }

                        throw new EndpointException(null, sb.ToString(), ex);
                    }
                }


			    _initialized = true;
			}
		}

		public IEndpoint Resolve(Uri uri)
		{
			Check.Parameter(uri).WithMessage("Uri").IsNotNull();

			if (_endpointTypes.Count == 0)
				Initialize();

			if (_cache.ContainsKey(uri))
				return _cache[uri];

			IEndpoint result = null;

			object[] args = new object[] {uri};

			string scheme = uri.Scheme;

			result = LookupByScheme(uri, scheme, args);

			if (result == null)
			{
				result = LookupByEndpointType(scheme, args);
			}

			if (result == null)
				throw new EndpointException(default(IEndpoint), "The endpoint address could not be resolved: " + uri);

			lock (_cache)
			{
				if (!_cache.ContainsKey(uri))
					_cache.Add(uri, result);
			}

			return result;
		}

		private IEndpoint LookupByScheme(Uri uri, string scheme, object[] args)
		{
			lock (_schemeTypes)
			{
				if (_schemeTypes.ContainsKey(scheme))
				{
					IEndpoint endpoint = (IEndpoint) Activator.CreateInstance(_schemeTypes[scheme], args);

					return endpoint;
				}
			}
			return null;
		}

		private IEndpoint LookupByEndpointType(string scheme, object[] args)
		{
			foreach (Type type in _endpointTypes)
			{
				try
				{
					IEndpoint endpoint = (IEndpoint) Activator.CreateInstance(type, args);

					lock (_schemeTypes)
					{
						if (!_schemeTypes.ContainsKey(scheme))
							_schemeTypes[scheme] = type;
					}

					return endpoint;
				}
				catch (Exception)
				{
					if (_log.IsDebugEnabled)
						_log.DebugFormat("The type {0} does not support the Uri scheme of {1}", type.FullName, scheme);
				}
			}

			return null;
		}
	}
}