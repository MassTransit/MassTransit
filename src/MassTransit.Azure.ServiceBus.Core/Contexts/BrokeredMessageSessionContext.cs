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
namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.ServiceBus;


    public class BrokeredMessageSessionContext :
        MessageSessionContext
    {
        readonly IMessageSession _session;

        public BrokeredMessageSessionContext(IMessageSession session)
        {
            _session = session;
        }

        public Task<byte[]> GetStateAsync()
        {
            return _session.GetStateAsync();
        }

        public Task SetStateAsync(byte[] sessionState)
        {
            return _session.SetStateAsync(sessionState);
        }

        public Task RenewLockAsync(Message message)
        {
            return _session.RenewLockAsync(message);
        }

        public DateTime LockedUntilUtc => _session.LockedUntilUtc;

        public string SessionId => _session.SessionId;
    }
}
