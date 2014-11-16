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


    public interface SentMessage
    {
        ISendContext Context { get; }
        Exception Exception { get; }

        Type MessageType { get; }
    }


    public interface SentMessage<out T> :
        SentMessage
        where T : class
    {
        new ISendContext<T> Context { get; }
    }


    public interface MessageSent
    {
        SendContext Context { get; }

        Exception Exception { get; }

        Type MessageType { get; }
    }


    public interface MessageSent<out T> :
        MessageSent
        where T : class
    {
        new SendContext<T> Context { get; }
    }


    class MessageSentImpl<T> :
        MessageSent<T>
        where T : class
    {
        readonly SendContext<T> _context;
        readonly Exception _exception;

        public MessageSentImpl(SendContext<T> context, Exception exception = null)
        {
            _context = context;
            _exception = exception;
        }

        SendContext MessageSent.Context
        {
            get { return _context; }
        }

        SendContext<T> MessageSent<T>.Context
        {
            get { return _context; }
        }

        Exception MessageSent.Exception
        {
            get { return _exception; }
        }

        Type MessageSent.MessageType
        {
            get { return typeof(T); }
        }
    }
}