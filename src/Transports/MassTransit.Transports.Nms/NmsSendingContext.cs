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
namespace MassTransit.Transports.Nms
{
    using System;
    using System.IO;
    using System.Text;
    using Apache.NMS;
    using Internal;

    public class NmsSendingContext :
        ISendingContext
    {
        readonly IMessageProducer _msgProducer;
        readonly ITextMessage _msg;

        public NmsSendingContext(IMessageProducer msgProducer)
        {
            _msgProducer = msgProducer;
            _msg = _msgProducer.CreateTextMessage();
        }

        public Stream Body { get; set; }

        public void MarkRecoverable()
        {
            _msg.NMSDeliveryMode = MsgDeliveryMode.Persistent;
        }

        public void SetLabel(string label)
        {
            throw new NotImplementedException();
        }

        public void SetMessageExpiration(DateTime d)
        {
            if (OutboundMessage.Headers.ExpirationTime.HasValue)
            {
                _msg.NMSTimeToLive = d - DateTime.UtcNow;
            }
        }

        public IMessage GetMessage()
        {
            string text = Encoding.UTF8.GetString(((MemoryStream) Body).ToArray());
            _msg.Text = text;
            return _msg;
        }
    }
}