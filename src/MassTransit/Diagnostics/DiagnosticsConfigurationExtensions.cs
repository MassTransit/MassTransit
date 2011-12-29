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
namespace MassTransit
{
    using System;
    using BusConfigurators;
    using BusServiceConfigurators;
    using Diagnostics.Introspection;
    using Diagnostics.Tracing;
    using Magnum.FileSystem;

    public static class DiagnosticsConfigurationExtensions
	{
        public static void EnableRemoteIntrospection(this ServiceBusConfigurator configurator)
        {
            var serviceConfigurator = new IntrospectionServiceConfigurator();

            var busConfigurator = new CustomBusServiceConfigurator(serviceConfigurator);

            configurator.AddBusConfigurator(busConfigurator);
        }

		public static void EnableMessageTracing(this ServiceBusConfigurator configurator)
		{
			var busConfigurator = new PostCreateBusBuilderConfigurator(bus =>
				{
					var service = new MessageTraceBusService(bus.EventChannel);

					bus.AddService(BusServiceLayer.Network, service);
				});

			configurator.AddBusConfigurator(busConfigurator);
		}


        //convenience methods

        public static void WriteIntrospectionToConsole(this IServiceBus bus)
        {
            var probe = bus.Probe();
            Console.Write(probe);
        }

        public static void WriteIntrospectionToFile(this IServiceBus bus, string fileName)
        {
            var probe = bus.Probe();
            var fs = new DotNetFileSystem();
            fs.DeleteFile(fileName);
            fs.Write(fileName, probe.ToString());

        }

        /// <summary>
        /// A convenience method for inspecting an active service bus instance.
        /// </summary>
        public static DiagnosticsProbe Probe(this IServiceBus bus)
        {
            var probe = new InMemoryDiagnosticsProbe();
            bus.Inspect(probe);
            return probe;
        }


	}
}