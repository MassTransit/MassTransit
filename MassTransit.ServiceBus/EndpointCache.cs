namespace MassTransit.ServiceBus
{
	using System;
	using System.Collections.Generic;

	public class EndpointCache
	{
		private readonly Dictionary<string, Type> _endpointTypes = new Dictionary<string, Type>();

		public int Count
		{
			get { return _endpointTypes.Count; }
		}

		public void Add(IEndpoint endpoint)
		{
			if (endpoint != null)
			{
				if (endpoint.Uri != null)
				{
					string scheme = endpoint.Uri.Scheme;

					lock (_endpointTypes)
					{
						if (!_endpointTypes.ContainsKey(scheme))
							_endpointTypes.Add(scheme, endpoint.GetType());
					}
				}
			}
		}

		public T Resolve<T>(Uri uri)
		{
			string scheme = uri.Scheme;
			if (_endpointTypes.ContainsKey(scheme))
			{
				return (T) Activator.CreateInstance(_endpointTypes[scheme], new object[] {uri});
			}

			return default(T);
		}
	}
}