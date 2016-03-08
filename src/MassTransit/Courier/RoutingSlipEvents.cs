// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Context;
    using Contracts;


    public static class RoutingSlipEventCorrelation
    {
        static readonly object _lock = new object();
        static bool _configured;

        public static void ConfigureMessageCorrelation()
        {
            lock (_lock)
            {
                if (_configured)
                    return;
                MessageCorrelation.UseCorrelationId<RoutingSlipCompleted>(x => x.TrackingNumber);
                MessageCorrelation.UseCorrelationId<RoutingSlipFaulted>(x => x.TrackingNumber);
                MessageCorrelation.UseCorrelationId<RoutingSlipActivityCompleted>(x => x.ExecutionId);
                MessageCorrelation.UseCorrelationId<RoutingSlipActivityFaulted>(x => x.ExecutionId);
                MessageCorrelation.UseCorrelationId<RoutingSlipActivityCompensated>(x => x.ExecutionId);
                MessageCorrelation.UseCorrelationId<RoutingSlipActivityCompensationFailed>(x => x.ExecutionId);
                MessageCorrelation.UseCorrelationId<RoutingSlipCompensationFailed>(x => x.TrackingNumber);
                MessageCorrelation.UseCorrelationId<RoutingSlipTerminated>(x => x.TrackingNumber);
                MessageCorrelation.UseCorrelationId<RoutingSlipRevised>(x => x.TrackingNumber);

                _configured = true;
            }
        }
    }
}