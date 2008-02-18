using System;
using System.Collections.Generic;
using MassTransit.ServiceBus;

namespace MassTransit.Host.Config
{
	public interface IEndpointConfigurator
	{
		IEndpoint Endpoint { get; }

		IList<Type> Types { get;  }
	}
}