// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Context
{
    using System;
    using System.IO;
    using System.Threading;


    public class MoveMessageSendContext 
    {
        readonly Action<Stream> _bodyWriter;
        readonly Action<EndpointAddress> _notifySend;

        public MoveMessageSendContext(ReceiveContext context)
        {
//            SetUsing(context);
            CopyOrInitializeOriginalMessageId(context);

          //  Id = context.Id;

        //    _notifySend = address => context.NotifySend(this, address);

          //  _bodyWriter = stream => context.CopyBodyTo(stream);
        }

        public Guid Id { get; set; }


        void CopyOrInitializeOriginalMessageId(ReceiveContext context)
        {
//            SetOriginalMessageId(context.OriginalMessageId);
//
//            if (string.IsNullOrEmpty(OriginalMessageId))
//                SetOriginalMessageId(context.MessageId);
        }

        public CancellationToken CancellationToken
        {
            get { throw new NotImplementedException(); }
        }

        public bool HasPayloadType(Type contextType)
        {
            throw new NotImplementedException();
        }

        public bool TryGetPayload<TPayload>(out TPayload payload) where TPayload : class
        {
            throw new NotImplementedException();
        }

        public TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory) where TPayload : class
        {
            throw new NotImplementedException();
        }

    }
}