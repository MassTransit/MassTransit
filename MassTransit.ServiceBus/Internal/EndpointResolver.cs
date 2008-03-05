using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;
using MassTransit.ServiceBus.Exceptions;
using MassTransit.ServiceBus.Util;

namespace MassTransit.ServiceBus.Internal
{
	public class EndpointResolver
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (EndpointResolver));

		private readonly List<Type> _endpointTypes = new List<Type>();
		private readonly Dictionary<string, Type> _schemeTypes = new Dictionary<string, Type>();
		private Assembly[] _assemblies;

		public void Initialize()
		{
			lock (_endpointTypes)
			{
				_endpointTypes.Clear();

				Type endpointType = typeof (IEndpoint);

				_assemblies = AppDomain.CurrentDomain.GetAssemblies();
				foreach (Assembly assembly in _assemblies)
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
			}
		}

		public IEndpoint Resolve(Uri uri)
		{
			Check.Parameter(uri).WithMessage("Uri").IsNotNull();

			if (_endpointTypes.Count == 0)
			{
				Initialize();
			}

			object[] args = new object[] {uri};

			string scheme = uri.Scheme;

			lock (_schemeTypes)
			{
				if (_schemeTypes.ContainsKey(scheme))
				{
					return (IEndpoint) Activator.CreateInstance(_schemeTypes[scheme], args);
				}
			}

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
				catch (Exception ex)
				{
					if (_log.IsDebugEnabled)
						_log.DebugFormat("The type {0} does not support the Uri scheme of {1}", type.FullName, scheme);
				}
			}

			throw new EndpointException(default(IEndpoint), "The endpoint address could not be resolved: " + uri);
		}
	}
}