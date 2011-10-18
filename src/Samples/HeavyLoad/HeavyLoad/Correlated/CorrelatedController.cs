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
namespace HeavyLoad.Correlated
{
    using System;
    using Magnum.Extensions;
    using MassTransit;

    internal class CorrelatedController
    {
        static readonly TimeSpan _timeout = 12.Seconds();
        readonly IServiceBus _bus;
        readonly Guid _id;
        readonly Action<CorrelatedController> _successAction;
        readonly Action<CorrelatedController> _timeoutAction;

        public CorrelatedController(IServiceBus bus,
                                    Action<CorrelatedController> successAction,
                                    Action<CorrelatedController> timeoutAction)
        {
            _bus = bus;
            _successAction = successAction;
            _timeoutAction = timeoutAction;

            _id = Guid.NewGuid();
        }

        public void SimulateRequestResponse()
        {
            _bus.PublishRequest(new SimpleRequestMessage(_id), x =>
                {
                    x.Handle<SimpleResponseMessage>(message =>
                        {
                            if (message.CorrelationId != _id)
                                throw new ArgumentException("Unknown message response received");

                            _successAction(this);
                        });

                    x.HandleTimeout(_timeout, () =>
                        {
                            // we timed out, not so happy
                            _timeoutAction(this);
                        });
                });
        }
    }
}