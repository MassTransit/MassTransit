// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Distributor.Messages
{
    using System;

    /// <summary>
    ///   Wraps a message type as a distributed message so that it can be sent separately from the actual
    ///   message being processed. We intentionally make sure it is not assignable to TMessage, because we don't
    ///   want it getting downgraded to a consumer of TMessage.
    /// </summary>
    /// <typeparam name = "TMessage">The message type being distributed</typeparam>
	[Serializable]
    public class Distributed<TMessage> :
        CorrelatedBy<Guid>
    {
        public Distributed(TMessage message)
            :
                this(message, null)
        {
        }

        public Distributed(TMessage message, Uri responseAddress)
        {
            Payload = message;
            ResponseAddress = responseAddress;
            CorrelationId = NewId.NextGuid();
        }

        protected Distributed()
        {
        }

        public TMessage Payload { get; set; }
        public Uri ResponseAddress { get; set; }
        public Guid CorrelationId { get; set; }
    }
}