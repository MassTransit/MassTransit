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
namespace MassTransit.AzureServiceBusTransport.Contexts
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
#if !NETCORE
    using Microsoft.ServiceBus.Messaging;
#endif

    public class BrokeredMessageSessionContext :
        MessageSessionContext
    {
        readonly MessageSession _session;

        public BrokeredMessageSessionContext(MessageSession session)
        {
            _session = session;
        }

        public Task<Stream> GetStateAsync()
        {
            return _session.GetStateAsync();
        }

        public Task SetStateAsync(Stream sessionState)
        {
            return _session.SetStateAsync(sessionState);
        }

        public Task RenewLockAsync()
        {
            return _session.RenewLockAsync();
        }

        public DateTime LockedUntilUtc => _session.LockedUntilUtc;

        public string SessionId => _session.SessionId;
    }
}