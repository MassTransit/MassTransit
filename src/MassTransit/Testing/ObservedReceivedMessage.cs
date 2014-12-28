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


    public class ObservedReceivedMessage<T> :
        IReceivedMessage<T>
        where T : class
    {
        readonly ConsumeContext<T> _context;
        readonly Exception _exception;

        public ObservedReceivedMessage(ConsumeContext<T> context, Exception exception = null)
        {
            _context = context;
            _exception = exception;
        }

        ConsumeContext IReceivedMessage.Context
        {
            get { return _context; }
        }

        public Exception Exception
        {
            get { return _exception; }
        }

        public Type MessageType
        {
            get { return typeof(T); }
        }

        public ConsumeContext<T> Context
        {
            get { return _context; }
        }
    }
}