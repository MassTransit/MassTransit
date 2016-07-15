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
namespace MassTransit.Scheduling
{
    using System;


    public class ScheduledMessageHandle<T> :
        ScheduledMessage<T>
        where T : class
    {
        public ScheduledMessageHandle(Guid tokenId, DateTime scheduledTime, Uri destination, T payload)
        {
            TokenId = tokenId;
            ScheduledTime = scheduledTime;
            Destination = destination;
            Payload = payload;
        }

        public Guid TokenId { get; }
        public DateTime ScheduledTime { get; }
        public Uri Destination { get; }
        public T Payload { get; }
    }
}