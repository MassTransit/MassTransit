// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System;
    using System.IO;
    using Hosting;
    using MassTransit.Context;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;


    public class HttpReceiveContext :
        BaseReceiveContext
    {
        readonly HttpContext _httpContext;
        byte[] _body;

        public HttpReceiveContext(HttpContext httpContext, bool redelivered, IReceiveObserver receiveObserver, ReceiveEndpointContext topology)
            : base(new Uri(httpContext.Request.GetDisplayUrl()), redelivered, receiveObserver, topology)
        {
            _httpContext = httpContext;

            HeaderProvider = new HttpHeaderProvider(httpContext.Request.Headers);
        }

        protected override IHeaderProvider HeaderProvider { get; }

        public HttpContext HttpContext => _httpContext;

        public override byte[] GetBody()
        {
            if (_body == null)
                GetBodyAsByteArray();

            return _body;
        }

        public override Stream GetBodyStream()
        {
            if (_body == null)
                GetBodyAsByteArray();

            return new MemoryStream(_body, false);
        }

        void GetBodyAsByteArray()
        {
            using (var ms = new MemoryStream())
            {
                _httpContext.Request.Body.CopyTo(ms);

                _body = ms.ToArray();
            }
        }
    }
}