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
namespace MassTransit.Testing.MessageObservers
{
    using System;


    public class PublishedMessage<T> :
        IPublishedMessage<T>
        where T : class
    {
        readonly PublishContext<T> _context;

        public PublishedMessage(PublishContext<T> context, Exception exception = null)
        {
            _context = context;
            Exception = exception;
        }

        public PublishContext<T> Context => _context;

        SendContext IPublishedMessage.Context => Context;

        public Exception Exception { get; }

        public Type MessageType => typeof(T);

        public override int GetHashCode()
        {
            return _context?.GetHashCode() ?? 0;
        }

        public bool Equals(PublishedMessage<T> other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(other._context.Message, _context.Message);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof(PublishedMessage<T>))
                return false;
            return Equals((PublishedMessage<T>)obj);
        }
    }
}