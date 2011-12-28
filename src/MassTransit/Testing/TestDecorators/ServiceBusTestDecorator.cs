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
namespace MassTransit.Testing.TestDecorators
{
	using System;
	using Diagnostics;
	using Pipeline;
	using Scenarios;

	public class ServiceBusTestDecorator :
		IServiceBus
	{
		readonly IServiceBus _bus;
		readonly PublishedMessageListImpl _published;
		EndpointTestScenarioImpl _scenario;

		public ServiceBusTestDecorator(IServiceBus bus, EndpointTestScenarioImpl scenario)
		{
			_bus = bus;
			_scenario = scenario;

			_published = new PublishedMessageListImpl();
		}


	    public void Diagnose(DiagnosticsProbe probe)
	    {
	        _bus.Diagnose(probe);
	    }

	    public void Dispose()
		{
			_bus.Dispose();
		}

		public IEndpoint Endpoint
		{
			get { return _bus.Endpoint; }
		}

		public IInboundMessagePipeline InboundPipeline
		{
			get { return _bus.InboundPipeline; }
		}

		public IOutboundMessagePipeline OutboundPipeline
		{
			get { return _bus.OutboundPipeline; }
		}

		public IServiceBus ControlBus
		{
			get { return _bus.ControlBus; }
		}

		public IEndpointCache EndpointCache
		{
			get { return _bus.EndpointCache; }
		}

		public void Publish<T>(T message, Action<IPublishContext<T>> contextCallback)
			where T : class
		{
			PublishedMessageImpl<T> published = null;
			try
			{
				_bus.Publish(message, context =>
					{
						published = new PublishedMessageImpl<T>(context);

						contextCallback(context);
					});
			}
			catch (Exception ex)
			{
				if (published != null)
					published.SetException(ex);
				throw;
			}
			finally
			{
				if (published != null)
				{
					_published.Add(published);
					_scenario.AddPublished(published);
				}
			}
		}

		public IEndpoint GetEndpoint(Uri address)
		{
			return _bus.GetEndpoint(address);
		}

		public UnsubscribeAction Configure(Func<IInboundPipelineConfigurator, UnsubscribeAction> configure)
		{
			return _bus.Configure(configure);
		}
	}
}