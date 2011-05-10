// Copyright 2007-2010 The Apache Software Foundation.
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
namespace MassTransit.Serialization
{
    using System;
    using MessageHeaders;

	public class JsonMessageEnvelope :
        MessageEnvelopeBase
    {
        public JsonMessageEnvelope()
        {
        }

        JsonMessageEnvelope(Type messageType, object message)
        {
            Message = message;
            MessageType = messageType.ToMessageName();

            this.CopyFrom(OutboundMessage.Headers);
        }

		public object Message { get; set; }

        public static JsonMessageEnvelope Create<T>(object message)
        {
            return new JsonMessageEnvelope(typeof (T), message);
        }
    }
}