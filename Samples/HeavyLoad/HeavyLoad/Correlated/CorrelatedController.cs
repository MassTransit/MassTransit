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
namespace HeavyLoad.Correlated
{
    using System;
    using MassTransit;

    internal class CorrelatedController :
        Consumes<SimpleResponseMessage>.For<Guid>
    {
        private static readonly TimeSpan _timeout = TimeSpan.FromSeconds(6);
        private readonly IServiceBus _bus;
        private readonly Guid _id;
        private readonly Action<CorrelatedController> _successAction;
        private readonly Action<CorrelatedController> _timeoutAction;
        private ServiceBusRequest<CorrelatedController> _request;

        public CorrelatedController(IServiceBus bus, Action<CorrelatedController> successAction, Action<CorrelatedController> timeoutAction)
        {
            _bus = bus;
            _successAction = successAction;
            _timeoutAction = timeoutAction;

            _id = Guid.NewGuid();
        }

        public void Consume(SimpleResponseMessage message)
        {
            if (message.CorrelationId != _id)
                throw new ArgumentException("Unknown message response received");

            _request.Complete();
        }

        public Guid CorrelationId
        {
            get { return _id; }
        }

        public void SimulateRequestResponse()
        {
            _request = _bus.Request().From(this);

            _request.Send(new SimpleRequestMessage(_id), RequestMode.Synchronous, _timeout);

            if (_request.IsCompleted)
                _successAction(this);
            else
                _timeoutAction(this);
        }
    }
}