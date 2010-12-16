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
namespace MassTransit.Internal
{
    using System;
    using System.Diagnostics;
    using Exceptions;
    using Common.Logging;

    [DebuggerDisplay("Poison:{Uri}")]
    public class PoisonEndpointDecorator :
        IEndpoint
    {
        private readonly ILog _log = LogManager.GetLogger(typeof (PoisonEndpointDecorator));
        private readonly IEndpoint _wrappedEndpoint;

        public PoisonEndpointDecorator(IEndpoint wrappedEndpoint)
        {
            _wrappedEndpoint = wrappedEndpoint;
        }

        public void Dispose()
        {
            _wrappedEndpoint.Dispose();
        }

        public IEndpointAddress Address
        {
            get { return _wrappedEndpoint.Address; }
        }

        public Uri Uri
        {
            get { return _wrappedEndpoint.Uri; }
        }

        public void Send<T>(T message) where T : class
        {
            if (_log.IsWarnEnabled)
                _log.WarnFormat("Saving Poison Message {0}", message.GetType());

            _wrappedEndpoint.Send(message);
        }

        public void Receive(Func<object, Action<object>> receiver)
        {
            throw new EndpointException(_wrappedEndpoint.Uri, "Receive from poison endpoint is not allowed");
        }

        public void Receive(Func<object, Action<object>> receiver, TimeSpan timeout)
        {
            throw new EndpointException(_wrappedEndpoint.Uri, "Receive from poison endpoint is not allowed");
        }
    }
}