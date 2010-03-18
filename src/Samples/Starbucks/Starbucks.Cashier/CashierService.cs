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
namespace Starbucks.Cashier
{
    using MassTransit;

    public class CashierService
    {
        IServiceBus _bus;
        UnsubscribeAction _unsubscribeAction;

        public CashierService(IServiceBus bus)
        {
            _bus = bus;
        }

        public void Start()
        {
            _unsubscribeAction = _bus.Subscribe<CashierSaga>();
        }

        public void Stop()
        {
            _unsubscribeAction();
            _bus.Dispose();
        }
    }
}