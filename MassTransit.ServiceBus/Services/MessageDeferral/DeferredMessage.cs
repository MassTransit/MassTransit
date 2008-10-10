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
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    public class DeferredMessage
    {
        private static readonly BinaryFormatter _formatter = new BinaryFormatter();

        private readonly DateTime _deliverAt;
        private readonly Guid _id;
        private readonly byte[] _messageData;

        public DeferredMessage(Guid id, DateTime deliverAt, object message)
        {
            _id = id;
            _deliverAt = deliverAt;
            _messageData = SerializeMessage(message);
        }

        protected DeferredMessage()
        {
        }


        public Guid Id
        {
            get { return _id; }
        }

        public DateTime DeliverAt
        {
            get { return _deliverAt; }
        }

        private static byte[] SerializeMessage(object message)
        {
            using (MemoryStream mstream = new MemoryStream())
            {
                _formatter.Serialize(mstream, message);
                return mstream.ToArray();
            }
        }

        public T GetMessage<T>() where T : class
        {
            return GetMessage() as T;
        }

        public object GetMessage()
        {
            using (MemoryStream mstream = new MemoryStream(_messageData))
            {
                object obj = _formatter.Deserialize(mstream);

                return obj;
            }
        }
    }
}