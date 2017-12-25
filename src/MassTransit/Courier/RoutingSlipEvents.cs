// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Courier
{
    using Contracts;
    using Topology;
    using Topology.Topologies;


    public static class RoutingSlipEventCorrelation
    {
        static readonly object _lock = new object();
        static bool _configured;

        public static void ConfigureCorrelationIds()
        {
            lock (_lock)
            {
                if (_configured)
                    return;

                ConfigureCorrelationIds(GlobalTopology.Send);

                _configured = true;
            }
        }

        public static void ConfigureCorrelationIds(ISendTopology topology)
        {
            topology.UseCorrelationId<RoutingSlipCompleted>(x => x.TrackingNumber);
            topology.UseCorrelationId<RoutingSlipFaulted>(x => x.TrackingNumber);
            topology.UseCorrelationId<RoutingSlipActivityCompleted>(x => x.ExecutionId);
            topology.UseCorrelationId<RoutingSlipActivityFaulted>(x => x.ExecutionId);
            topology.UseCorrelationId<RoutingSlipActivityCompensated>(x => x.ExecutionId);
            topology.UseCorrelationId<RoutingSlipActivityCompensationFailed>(x => x.ExecutionId);
            topology.UseCorrelationId<RoutingSlipCompensationFailed>(x => x.TrackingNumber);
            topology.UseCorrelationId<RoutingSlipTerminated>(x => x.TrackingNumber);
            topology.UseCorrelationId<RoutingSlipRevised>(x => x.TrackingNumber);
        }
    }
}