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


    public class ObservedSentMessage<T> :
        ISentMessage<T>
        where T : class
    {
        readonly SendContext<T> _context;
        readonly Exception _exception;

        public ObservedSentMessage(SendContext<T> context, Exception exception = null)
        {
            _context = context;
            _exception = exception;
        }

        SendContext ISentMessage.Context
        {
            get { return _context; }
        }

        SendContext<T> ISentMessage<T>.Context
        {
            get { return _context; }
        }

        Exception ISentMessage.Exception
        {
            get { return _exception; }
        }

        Type ISentMessage.MessageType
        {
            get { return typeof(T); }
        }
    }
}