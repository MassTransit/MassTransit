using System;
using System.Collections.Generic;
using MassTransit.ServiceBus;

namespace MassTransit.Host.Config
{
	public class EndpointConfigurator :
		IEndpointConfigurator
	{
		private readonly IEndpoint _endpoint;

		private readonly List<Type> _types = new List<Type>();

		public EndpointConfigurator(IEndpoint endpoint)
		{
			_endpoint = endpoint;
		}

		#region IEndpointConfigurator Members

		public IEndpoint Endpoint
		{
			get { return _endpoint; }
		}

		public IList<Type> Types
		{
			get { return _types; }
		}

		#endregion
	}
}