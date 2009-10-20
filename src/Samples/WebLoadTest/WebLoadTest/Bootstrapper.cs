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
	using Actors;
	using MassTransit.Actors;
	using MassTransit.Saga;
	using StructureMap;
	using StructureMap.Attributes;

	public static class Bootstrapper
	{
		public static void Bootstrap()
		{
			BootstrapContainer();

			ActorBootstrapper.Bootstrap();
		}

		private static void BootstrapContainer()
		{
			var repository = new InMemoryActorRepository<StockQuoteRequestActor>();

			ObjectFactory.Configure(x =>
				{
					x.AddRegistry(new WebLoadTestRegistry());

					x.For<IActorRepository<StockQuoteRequestActor>>()
						.Use(repository);

					x.For<ISagaRepository<StockQuoteRequestActor>>()
						.Use(repository);

					x.For<StockQuoteRequestActor>()
						.CacheBy(InstanceScope.HttpContext);
				});
		}
	}
}