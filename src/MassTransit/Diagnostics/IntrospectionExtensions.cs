// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Diagnostics.Introspection;


    public static class IntrospectionExtensions
    {
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