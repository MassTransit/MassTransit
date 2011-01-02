// Copyright 2007-2011 The Apache Software Foundation.
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
namespace MassTransit.Transports.Msmq
{
    using System;
    using System.IO;
    using System.Messaging;

    public class MsmqSendingContext :
        ISendingContext
    {
        readonly Message _msg;

        public MsmqSendingContext(Message msg)
        {
            _msg = msg;
        }

        public Stream Body
        {
            get { return _msg.BodyStream; }
            set { _msg.BodyStream = value; }
        }

        public void MarkRecoverable()
        {
            _msg.Recoverable = true;
        }

        public void SetLabel(string label)
        {
            _msg.Label = label;
        }

        public void SetMessageExpiration(DateTime d)
        {
            //_msg.TimeToBeReceived = d;
        }
    }
}