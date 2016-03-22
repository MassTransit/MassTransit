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
    using MassTransit.Pipeline;
    using Owin;


    public class HttpMassTransitStartUp
    {
        readonly IReceiveObserver _observer;
        readonly IPipe<ReceiveContext> _receivePipe;

        public HttpMassTransitStartUp(IPipe<ReceiveContext> receivePipe)
        {
            _receivePipe = receivePipe;
            _observer = new ReceiveObservable();
        }

        // ReSharper disable once UnusedMember.Local - called from within WebApp.Start
        public void Configuration(IAppBuilder app)
        {
            app.Use(async (cxt, next) =>
            {
                var inputAddress = cxt.Request.Uri;
                var body = cxt.Request.Body;
                var headers = new HttpHeaderProvider(cxt.Request.Headers);
                if (_receivePipe != null)
                    await _receivePipe.Send(new HttpReceiveContext(inputAddress, body, headers, false, _observer)).ConfigureAwait(false);
                cxt.Response.Write("DELIVERED");
                await next();
            });
        }
    }
}