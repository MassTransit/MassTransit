// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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


    static class ScheduledMessageToken
    {
        internal static readonly ulong Tag;
        internal static readonly byte[] Key;

        static ScheduledMessageToken()
        {
            var guid = new Guid("E25FC12B-FF28-4476-A6E1-DE45E154A675");

            Key = guid.ToByteArray();

            Tag = BitConverter.ToUInt64(Key, 8);
        }
    }


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
            byte[] key = ScheduledMessageToken.Key;

            var bytes = new byte[16];
            Buffer.BlockCopy(key, 8, bytes, 8, 8);

            byte[] sequenceNumberBytes = BitConverter.GetBytes(sequenceNumber);

            var sequenceLength = sequenceNumberBytes.Length;

            Buffer.BlockCopy(sequenceNumberBytes, 0, bytes, 0, sequenceLength);

            ScheduledMessageId = new Guid(bytes);
        }

        public bool TryGetScheduledMessageId(out long sequenceNumber)
        {
            if (ScheduledMessageId.HasValue)
                return TryGetSequenceNumber(ScheduledMessageId.Value, out sequenceNumber);

            sequenceNumber = 0;
            return false;
        }

        public bool TryGetSequenceNumber(Guid id, out long sequenceNumber)
        {
            byte[] bytes = id.ToByteArray();

            if (BitConverter.ToUInt64(bytes, 8) == ScheduledMessageToken.Tag)
            {
                sequenceNumber = BitConverter.ToInt64(bytes, 0);
                return true;
            }

            sequenceNumber = default;
            return false;
        }

        public long GetSequenceNumber(Guid scheduledMessageId)
        {
            return BitConverter.ToInt64(scheduledMessageId.ToByteArray(), 0);
        }
    }
}