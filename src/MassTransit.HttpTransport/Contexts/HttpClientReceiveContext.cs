// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Net.Http;
    using Context;
    using Hosting;
    using MassTransit.Topology;


    public class HttpClientReceiveContext :
        BaseReceiveContext
    {
        readonly HttpResponseMessage _responseMessage;
        readonly Stream _responseStream;

        public HttpClientReceiveContext(HttpResponseMessage responseMessage, Stream responseStream, bool redelivered, IReceiveObserver receiveObserver,
            IReceiveEndpointTopology topology)
            : base(responseMessage.RequestMessage.RequestUri, redelivered, receiveObserver, topology)
        {
            _responseMessage = responseMessage;
            _responseStream = responseStream;

            HeaderProvider = new HttpClientHeaderProvider(responseMessage.Headers);
        }

        protected override IHeaderProvider HeaderProvider { get; }

        public HttpResponseMessage ResponseMessage => _responseMessage;

        protected override Stream GetBodyStream()
        {
            return _responseStream;
        }
    }
}