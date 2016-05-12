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
namespace MassTransit.Events
{
    using System;
    using System.Linq;


    [Serializable]
    public class FaultEvent<T> :
        Fault<T>
    {
        public FaultEvent(T message, Guid? faultedMessageId, HostInfo host, Exception exception)
        {
            Timestamp = DateTime.UtcNow;
            FaultId = NewId.NextGuid();

            Message = message;
            Host = host;
            FaultedMessageId = faultedMessageId;

            var aggregateException = exception as AggregateException;
            Exceptions = aggregateException?.InnerExceptions
                .Where(x => x != null)
                .Select(x => (ExceptionInfo)new FaultExceptionInfo(x))
                .ToArray()
                ?? new ExceptionInfo[] {new FaultExceptionInfo(exception)};
        }

        public Guid FaultId { get; }
        public Guid? FaultedMessageId { get; }
        public DateTime Timestamp { get; }
        public ExceptionInfo[] Exceptions { get; }
        public HostInfo Host { get; }
        public T Message { get; }
    }
}