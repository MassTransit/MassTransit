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
namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Internals.Extensions;


    [Serializable]
    public class PublishException :
        MassTransitException
    {
        public PublishException()
        {
        }

        public PublishException(Type messageType, IEnumerable<Exception> exceptions)
            : base(CreateMessage(messageType, exceptions), exceptions.First())
        {
            MessageType = messageType;
        }

        protected PublishException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public Type MessageType { get; protected set; }

        static string CreateMessage(Type messageType, IEnumerable<Exception> exceptions)
        {
            SendException sendException = exceptions
                .Where(x => x.GetType() == typeof(SendException))
                .Cast<SendException>()
                .FirstOrDefault();

            if (sendException != null)
            {
                return $"At least one exception occurred publishing {sendException.MessageType.GetTypeName()} to {sendException.Uri}";
            }

            return $"At least one exception occurred publishing {messageType.GetTypeName()}";
        }
    }
}