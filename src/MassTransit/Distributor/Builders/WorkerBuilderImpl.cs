// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Distributor.Builders
{
    using System.Collections.Generic;
    using Connectors;

    public class WorkerBuilderImpl :
        WorkerBuilder
    {
        readonly IList<WorkerConnector> _connectors;

        public WorkerBuilderImpl()
        {
            _connectors = new List<WorkerConnector>();
        }

        public void Add(WorkerConnector builder)
        {
            _connectors.Add(builder);
        }

        public IBusService Build()
        {
            return new WorkerBusService(_connectors);
        }
    }
}