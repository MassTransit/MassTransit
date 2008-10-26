// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.ServiceBus.Services.MessageDeferral
{
    using System;
    using System.Collections.Generic;
    using Messages;

    public class RemoteMessageDeferralViewer :
        Consumes<NewDeferMessageReceived>.All,
        Consumes<DeferedMessageRepublished>.All
    {
        private IDictionary<Guid, NewDeferMessageReceived> _messages;

        public RemoteMessageDeferralViewer()
        {
            _messages = new Dictionary<Guid, NewDeferMessageReceived>();
        }

        public IList<NewDeferMessageReceived> Messages
        {
            get { return new List<NewDeferMessageReceived>(_messages.Values); }
        }

        void Consumes<NewDeferMessageReceived>.All.Consume(NewDeferMessageReceived message)
        {
            _messages.Add(message.Id, message);
        }

        void Consumes<DeferedMessageRepublished>.All.Consume(DeferedMessageRepublished message)
        {
            _messages.Remove(message.DeferredMessageId);
        }
    }
}