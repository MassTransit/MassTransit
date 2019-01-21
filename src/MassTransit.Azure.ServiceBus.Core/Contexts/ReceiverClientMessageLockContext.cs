// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Core;
    using Util;


    public struct ReceiverClientMessageLockContext :
        MessageLockContext
    {
        readonly IReceiverClient _receiverClient;
        readonly Message _message;

        public ReceiverClientMessageLockContext(IReceiverClient receiverClient, Message message)
        {
            _receiverClient = receiverClient;
            _message = message;
        }

        public DateTime LockedUntil => _message.SystemProperties.LockedUntilUtc;

        public Task Complete()
        {
            return _receiverClient.CompleteAsync(_message.SystemProperties.LockToken);
        }

        public Task Abandon(Exception exception)
        {
            return _receiverClient.AbandonAsync(_message.SystemProperties.LockToken, ExceptionUtil.GetExceptionHeaderDictionary(exception));
        }
    }
}
