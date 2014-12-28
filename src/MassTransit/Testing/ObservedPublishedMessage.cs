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
namespace MassTransit.Testing
{
    using System;


    public class ObservedPublishedMessage<T> :
        IPublishedMessage<T>
        where T : class
    {
        readonly PublishContext<T> _context;
        readonly Exception _exception;

        public ObservedPublishedMessage(PublishContext<T> context, Exception exception = null)
        {
            _context = context;
            _exception = exception;
        }

        public PublishContext<T> Context
        {
            get { return _context; }
        }

        SendContext IPublishedMessage.Context
        {
            get { return Context; }
        }

        public Exception Exception
        {
            get { return _exception; }
        }

        public Type MessageType
        {
            get { return typeof(T); }
        }

        public override int GetHashCode()
        {
            return (_context != null ? _context.GetHashCode() : 0);
        }

        public bool Equals(ObservedPublishedMessage<T> other)
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
            if (obj.GetType() != typeof(ObservedPublishedMessage<T>))
                return false;
            return Equals((ObservedPublishedMessage<T>)obj);
        }
    }
}