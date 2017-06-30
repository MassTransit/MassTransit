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
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class ReceivedMessageList :
        MessageList<IReceivedMessage>,
        IReceivedMessageList
    {
        public ReceivedMessageList(TimeSpan timeout)
            : base((int)timeout.TotalMilliseconds)
        {
        }

        public IEnumerable<IReceivedMessage<T>> Select<T>()
            where T : class
        {
            return Select(x => typeof(T).IsAssignableFrom(x.MessageType))
                .Cast<IReceivedMessage<T>>();
        }

        public IEnumerable<IReceivedMessage<T>> Select<T>(Func<IReceivedMessage<T>, bool> filter)
            where T : class
        {
            return Select(x => typeof(T).IsAssignableFrom(x.MessageType))
                .Cast<IReceivedMessage<T>>()
                .Where(filter);
        }

        public void Add<T>(ConsumeContext<T> context)
            where T : class
        {
            Add(new ReceivedMessage<T>(context), context.MessageId);
        }

        public void Add<T>(ConsumeContext<T> context, Exception exception)
            where T : class
        {
            Add(new ReceivedMessage<T>(context, exception), context.MessageId);
        }
    }


    public class ReceivedMessageList<T> :
        MessageList<IReceivedMessage<T>>,
        IReceivedMessageList<T>
        where T : class
    {
        public ReceivedMessageList(TimeSpan timeout)
            : base((int)timeout.TotalMilliseconds)
        {
        }

        public void Add(ConsumeContext<T> context)
        {
            Add(new ReceivedMessage<T>(context), context.MessageId);
        }

        public void Add(ConsumeContext<T> context, Exception exception)
        {
            Add(new ReceivedMessage<T>(context, exception), context.MessageId);
        }
    }
}