// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Context
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class PayloadException :
        MassTransitException
    {
        public PayloadException()
        {
        }

        public PayloadException(string message)
            : base(message)
        {
        }

        public PayloadException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected PayloadException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class PayloadFactoryException : 
        PayloadException
    {
        public PayloadFactoryException()
        {
        }

        public PayloadFactoryException(string message)
            : base(message)
        {
        }

        public PayloadFactoryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected PayloadFactoryException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }


    [Serializable]
    public class PayloadNotFoundException : 
        PayloadException
    {
        public PayloadNotFoundException()
        {
        }

        public PayloadNotFoundException(string message)
            : base(message)
        {
        }

        public PayloadNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected PayloadNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}