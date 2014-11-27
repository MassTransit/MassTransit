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
namespace MassTransit.Testing.Scenarios
{
    using System;


    public class BusTestScenarioImpl :
        EndpointTestScenarioImpl,
        BusTestScenario
    {
        IBusControl _bus;
        bool _disposed;

        public override IBus InputBus
        {
            get { return Bus; }
        }

        public override ISendEndpoint InputQueueSendEndpoint
        {
            get { return _bus.GetSendEndpoint(new Uri("loopback://localhost/input_queue")).Result; }
        }

        public IBus Bus
        {
            get { return _bus; }
            set
            {
                _bus = value as IBusControl;
                if (_bus == null)
                    throw new ArgumentException("The bus must be controllable");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (_bus != null)
                    _bus.Dispose();

                base.Dispose(true);
            }

            _disposed = true;
        }
    }
}