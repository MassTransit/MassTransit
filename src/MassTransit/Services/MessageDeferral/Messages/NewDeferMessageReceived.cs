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
namespace MassTransit.Services.MessageDeferral.Messages
{
    using System;

    [Serializable]
    public class NewDeferMessageReceived
    {
        private DateTime _deliverAt;
        private Guid _id;
        private string _messageType;

        protected NewDeferMessageReceived()
        {
        }

        public NewDeferMessageReceived(Guid id, DateTime deliverAt, string messageType)
        {
            _id = id;
            _deliverAt = deliverAt;
            _messageType = messageType;
        }

        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public DateTime DeliverAt
        {
            get { return _deliverAt; }
            set { _deliverAt = value; }
        }

        public string MessageType
        {
            get { return _messageType; }
            set { _messageType = value; }
        }
    }
}