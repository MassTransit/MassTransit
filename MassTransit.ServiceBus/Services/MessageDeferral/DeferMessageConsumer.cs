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
namespace MassTransit.ServiceBus.Services.MessageDeferral
{
    using System;
    using Messages;
    using Timeout.Messages;
    using Util;

    public class DeferMessageConsumer :
        Consumes<DeferMessage>.All
    {
        private readonly IServiceBus _bus;
        private readonly IDeferredMessageRepository _repository;

        public DeferMessageConsumer(IServiceBus bus, IDeferredMessageRepository repository)
        {
            _repository = repository;
            _bus = bus;
        }

        public void Consume(DeferMessage message)
        {
            Guid id = CombGuid.NewCombGuid();

            _repository.Add(new DeferredMessage(id, message.DeliverAt, message.Message));

            _bus.Publish(new ScheduleTimeout(id, message.DeliverAt));
        }
    }
}