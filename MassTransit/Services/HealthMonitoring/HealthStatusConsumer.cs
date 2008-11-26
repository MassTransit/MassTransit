// Copyright 2007-2008 The Apache Software Foundation.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//   http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Services.HealthMonitoring
{
    using System.Collections.Generic;
    using Messages;

    public class HealthStatusConsumer :
        Consumes<HealthStatusRequest>.All
    {
        private readonly IServiceBus _bus;
        private readonly IHealthCache _cache;

        public HealthStatusConsumer(IServiceBus bus, IHealthCache cache)
        {
            _bus = bus;
            _cache = cache;
        }

        #region Implementation of All

        public void Consume(HealthStatusRequest message)
        {
            IList<HealthInformation> _updates = _cache.List();
            _bus.Publish(new HealthStatusResponse(_updates, message.CorrelationId));
        }

        #endregion
    }
}