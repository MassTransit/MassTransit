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
namespace MassTransit.HttpTransport.Contexts
{
    using System.IO;
    using Context;
    using Microsoft.Owin;


    public class HttpReceiveContext :
        BaseReceiveContext
    {
        readonly IOwinContext _requestContext;

        public HttpReceiveContext(IOwinContext requestContext, IHeaderProvider provider, bool redelivered, IReceiveObserver receiveObserver,
            ISendEndpointProvider sendEndpointProvider, IPublishEndpointProvider publishEndpointProvider)
            : base(requestContext.Request.Uri, redelivered, receiveObserver, sendEndpointProvider, publishEndpointProvider)
        {
            _requestContext = requestContext;
            HeaderProvider = provider;
        }

        protected override IHeaderProvider HeaderProvider { get; }

        public IOwinContext RequestContext => _requestContext;

        protected override Stream GetBodyStream()
        {
            return _requestContext.Request.Body;
        }
    }
}