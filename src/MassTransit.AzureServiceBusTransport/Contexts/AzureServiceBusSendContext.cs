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
namespace MassTransit.AzureServiceBusTransport.Contexts
{
    using System;
    using System.Threading;
    using Context;


    public class AzureServiceBusSendContext<T> :
        BaseSendContext<T>,
        ServiceBusSendContext<T>
        where T : class
    {
        public AzureServiceBusSendContext(T message, CancellationToken cancellationToken)
            : base(message, cancellationToken)
        {
        }

        public string ReplyToSessionId { get; set; }

        public DateTime? ScheduledEnqueueTimeUtc { get; set; }

        public string PartitionKey { get; set; }

        public string SessionId { get; set; }

        public void SetScheduledMessageId(long sequenceNumber)
        {
            var id = NewId.NextGuid();

            byte[] bytes = id.ToByteArray();

            byte[] sequencyNumberBytes = BitConverter.GetBytes(sequenceNumber);

            Buffer.BlockCopy(sequencyNumberBytes, 0, bytes, 0, sequencyNumberBytes.Length);

            ScheduledMessageId = new Guid(bytes);
        }

        public bool TryGetScheduledMessageId(out long sequenceNumber)
        {
            if (ScheduledMessageId.HasValue)
            {
                sequenceNumber = GetSequenceNumber(ScheduledMessageId.Value);
                return true;
            }

            sequenceNumber = 0;
            return false;
        }

        public long GetSequenceNumber(Guid scheduledMessageId)
        {
            return BitConverter.ToInt64(scheduledMessageId.ToByteArray(), 0);
        }
    }
}