// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Steward.Core.Consumers
{
    using System;
    using Context;
    using Contracts;
    using Results;


    public class ConsumerDispatchContext :
        ConsumeContextProxy,
        DispatchContext
    {
        readonly DispatchMessage _message;

        public ConsumerDispatchContext(ConsumeContext<DispatchMessage> context, string body)
            : base(context)
        {
            _message = context.Message;

            Body = body;
        }

        public DispatchResult Accept()
        {
            return new AcceptDispatchResult();
        }

        public DispatchResult Delay(TimeSpan timeSpan, string reason)
        {
            return new DelayDispatchResult(timeSpan, reason);
        }

        public DispatchResult Discard(string reason)
        {
            return new DiscardDispatchResult(reason);
        }

        public DispatchResult Refuse(string reason)
        {
            return new RefuseDispatchResult(reason);
        }

        public DispatchResult Reject(string reason)
        {
            return new RejectDispatchResult(reason);
        }

        public string Body { get; }
        public Guid DispatchId => _message.DispatchId;
        public DateTime CreateTime => _message.CreateTime;
        public Uri[] Resources => _message.Resources;
        public string[] DispatchTypes => _message.PayloadType;
        public Uri Destination => _message.Destination;

        public bool TryGetContext<T>(out MessageDispatchContext<T> context)
            where T : class
        {
            context = null;
            return false;
        }
    }
}