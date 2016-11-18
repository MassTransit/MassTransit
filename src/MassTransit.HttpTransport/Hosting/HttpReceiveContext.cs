// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.HttpTransport.Hosting
{
    using System;
    using System.IO;
    using Context;


    public class HttpReceiveContext :
        BaseReceiveContext
    {
        readonly Stream _body;

        public HttpReceiveContext(Uri inputAddress, Stream body, IHeaderProvider provider, bool redelivered, IReceiveObserver receiveObserver, ISendEndpointProvider sendEndpointProvider, IPublishEndpointProvider publishEndpointProvider)
            : base(inputAddress, redelivered, receiveObserver, sendEndpointProvider, publishEndpointProvider)
        {
            _body = body;
            HeaderProvider = provider;
        }

        protected override IHeaderProvider HeaderProvider { get; }

        protected override Stream GetBodyStream()
        {
            return _body;
        }
    }
}