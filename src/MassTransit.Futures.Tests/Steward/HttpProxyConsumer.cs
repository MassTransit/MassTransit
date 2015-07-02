// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests.Steward
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using MassTransit.Steward;


    public class HttpProxyConsumer :
        IConsumer<ExecuteHttpRequest>
    {
        readonly HttpClient _client;

        public HttpProxyConsumer()
        {
            _client = new HttpClient();
        }

        async Task IConsumer<ExecuteHttpRequest>.Consume(ConsumeContext<ExecuteHttpRequest> context)
        {
            Console.WriteLine("Sending Request to {0}", context.Message.Url);

            DateTime startTime = DateTime.UtcNow;
            Stopwatch timer = Stopwatch.StartNew();

            using (HttpResponseMessage responseMessage =
                await _client.GetAsync(context.Message.Url, HttpCompletionOption.ResponseHeadersRead, context.CancellationToken))
            {
                timer.Stop();

                Console.WriteLine("Request completing as {0} ({1})", responseMessage.StatusCode, context.Message.Url);

                if (responseMessage.IsSuccessStatusCode)
                {
                    context.Respond(new HttpRequestSucceededEvent(responseMessage.StatusCode));

                    context.NotifyResourceUsageCompleted(context.Message.Url, startTime,
                        timer.Elapsed);
                }
                else
                {
                    context.Respond(new HttpRequestFaultedEvent(responseMessage.StatusCode));

                    context.NotifyResourceUsageFailed(context.Message.Url, startTime, timer.Elapsed, (int)responseMessage.StatusCode,
                        responseMessage.ReasonPhrase);
                }
            }

            context.Respond(new HttpRequestFaultedEvent(HttpStatusCode.InternalServerError));
        }


        public class HttpRequestFaultedEvent :
            HttpRequestFaulted
        {
            public HttpRequestFaultedEvent(HttpStatusCode statusCode)
            {
                StatusCode = (int)statusCode;
            }

            public int StatusCode { get; private set; }
        }


        public class HttpRequestSucceededEvent :
            HttpRequestSucceeded
        {
            public HttpRequestSucceededEvent(HttpStatusCode statusCode)
            {
                StatusCode = (int)statusCode;
            }

            public int StatusCode { get; private set; }
        }
    }


    public interface HttpRequestSucceeded
    {
        int StatusCode { get; }
    }


    public interface HttpRequestFaulted
    {
        int StatusCode { get; }
    }


    public interface ExecuteHttpRequest
    {
        Uri Url { get; }
    }
}