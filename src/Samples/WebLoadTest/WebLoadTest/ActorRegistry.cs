// Copyright 2007-2008 The Apache Software Foundation.
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
namespace WebLoadTest
{
	using System;
	using System.Web.Routing;
	using Actors;
	using Magnum;
	using Magnum.Actors;
	using Magnum.DateTimeExtensions;
	using MassTransit;
	using MassTransit.Pipeline.Inspectors;
	using StructureMap;

	public static class ActorBootstrapper
	{
		public static void Bootstrap()
		{
			RegisterServiceBus();

			var bus = ObjectFactory.GetInstance<IServiceBus>();
			bus.Subscribe<RequestStockQuote>(request =>
				{
					//ThreadUtil.Sleep(20.Milliseconds());

					bus.Publish(new StockQuoteReceivedImpl
						{
							Symbol = request.Symbol,
							Bid = 200.00m,
							Ask = 201.00m,
							Last = 200.50m,
						});
				});

			bus.Subscribe<StockQuoteRequestActor>();

			RegisterRoutes();

			PipelineViewer.Trace(bus.InboundPipeline);
		}

		private static void RegisterServiceBus()
		{
			new WebLoadTestRegistry();
		}

		private static void RegisterRoutes()
		{
			WithEach<AsyncHttpActor>(type =>
				{
					string routeUrl = type.Name;
					if (routeUrl.EndsWith("Actor"))
						routeUrl = "Actors/" + routeUrl.Substring(0, routeUrl.Length - 5);

					var route = new Route(routeUrl, new ActorRouteHandler(type));

					RouteTable.Routes.Add(route);
				});
		}


		public static void WithEach<T>(Action<Type> action)
		{
			foreach (var configuration in ObjectFactory.Model.PluginTypes)
			{
				if (!configuration.Implements<T>()) continue;

				action(configuration.PluginType);
			}
		}

		public static bool Implements<T>(this PluginTypeConfiguration configuration)
		{
			return configuration.PluginType.Implements<T>();
		}

		public static bool Implements<T>(this Type type)
		{
			return typeof (T).IsAssignableFrom(type);
		}
	}
}