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
namespace MassTransit.HttpTransport.Tests
{
    namespace HostingRequestResponse
    {
        using System;
        using System.Collections.Generic;
        using System.Diagnostics;
        using System.Net.Http;
        using System.Net.Http.Headers;
        using System.Text;
        using System.Threading.Tasks;
        using Metadata;
        using Newtonsoft.Json;
        using NUnit.Framework;
        using Serialization;


        [TestFixture]
        public class HostingRequestResponse_Specs :
            HttpTestFixture
        {
            [Test]
            public async Task Should_create_a_host_that_responds_to_requests()
            {
                using (var client = new HttpClient())
                {
                    var request = new Request {Value = "Hello"};
                    var envelope = new HttpMessageEnvelope(request, TypeMetadataCache<Request>.MessageTypeNames);
                    envelope.RequestId = NewId.NextGuid().ToString();
                    envelope.DestinationAddress = HostAddress.ToString();
                    envelope.ResponseAddress = new Uri("reply://localhost:8080/").ToString();

                    var messageBody = JsonConvert.SerializeObject(envelope, JsonMessageSerializer.SerializerSettings);

                    for (var i = 0; i < 5; i++)
                    {
                        var content = new StringContent(messageBody, Encoding.UTF8, "application/vnd.masstransit+json");

                        var timer = Stopwatch.StartNew();

                        using (var result = await client.PostAsync(HostAddress, content))
                        {
                            var response = await result.Content.ReadAsStringAsync();

                            if (!result.IsSuccessStatusCode)
                                await Console.Out.WriteAsync(response);

                            result.EnsureSuccessStatusCode();
                        }

                        timer.Stop();

                        await Console.Out.WriteLineAsync($"Request complete: {timer.ElapsedMilliseconds}ms");
                    }
                }
            }

            protected override void ConfigureHttpReceiveEndpoint(IHttpReceiveEndpointConfigurator configurator)
            {
                configurator.Consumer<HttpRequestConsumer>();
            }
        }


        [TestFixture]
        public class Sending_a_request_just_like_the_transport :
            HttpTestFixture
        {
            [Test]
            public async Task Should_create_a_host_that_responds_to_requests()
            {
                using (var client = new HttpClient())
                {
                    var request = new Request {Value = "Hello"};
                    var envelope = new HttpMessageEnvelope(request, TypeMetadataCache<Request>.MessageTypeNames);
                    envelope.RequestId = NewId.NextGuid().ToString();
                    envelope.DestinationAddress = HostAddress.ToString();
                    envelope.ResponseAddress = new Uri("reply://localhost:8080/").ToString();

                    var messageBody = JsonConvert.SerializeObject(envelope, JsonMessageSerializer.SerializerSettings);

                    for (var i = 0; i < 5; i++)
                    {
                        //                        var content = new StringContent(messageBody, Encoding.UTF8, "application/vnd.masstransit+json");

                        var content = new ByteArrayContent(Encoding.UTF8.GetBytes(messageBody));
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.masstransit+json");


                        var timer = Stopwatch.StartNew();

                        var message = new HttpRequestMessage(HttpMethod.Post, HostAddress);
                        message.Content = content;


                        message.Headers.Add(Clients.HttpHeaders.RequestId, envelope.RequestId);


                        using (var result = await client.SendAsync(message))
                        {
                            result.EnsureSuccessStatusCode();

                            await result.Content.ReadAsStringAsync();
                        }

                        timer.Stop();

                        await Console.Out.WriteLineAsync($"Request complete: {timer.ElapsedMilliseconds}ms");
                    }
                }
            }

            protected override void ConfigureHttpReceiveEndpoint(IHttpReceiveEndpointConfigurator configurator)
            {
                configurator.Consumer<HttpRequestConsumer>();
            }
        }


        [TestFixture]
        public class HostingRequestResponse2_Specs :
            HttpTestFixture
        {
            [Test]
            public async Task Should_work_with_api_receive_endpoint()
            {
                IRequestClient<Request, Response> client = HttpTestHarness.CreateRequestClient<Request, Response>(new Uri(HostAddress, "/api"));

                var request = new Request {Value = "Hello"};


                Stopwatch timer;
                Response result;
                for (var i = 0; i < 5; i++)
                {
                    timer = Stopwatch.StartNew();

                    result = await client.Request(request);

                    timer.Stop();

                    await Console.Out.WriteLineAsync($"Request complete: {timer.ElapsedMilliseconds}ms, Response = {result.ResponseValue}");
                }

                IRequestClient<Request, Response> rootClient = HttpTestHarness.CreateRequestClient<Request, Response>();

                timer = Stopwatch.StartNew();
                result = await rootClient.Request(request);
                timer.Stop();

                await Console.Out.WriteLineAsync($"Request complete: {timer.ElapsedMilliseconds}ms, Response = {result.ResponseValue}");
            }

            protected override void ConfigureHttpReceiveEndpoint(IHttpReceiveEndpointConfigurator configurator)
            {
                configurator.Consumer<HttpRequestConsumer>();
            }

            protected override void ConfigureHttpBusHost(IHttpBusFactoryConfigurator configurator, IHttpHost host)
            {
                configurator.ReceiveEndpoint(host, "/api", ep => ep.Consumer<HttpApiRequestConsumer>());
            }
        }


        [TestFixture]
        public class HostingRequestResponse3_Specs :
            HttpTestFixture
        {
            [Test]
            public async Task Should_work_with_the_message_request_client_too()
            {
                IRequestClient<Request, Response> client = HttpTestHarness.CreateRequestClient<Request, Response>(HostAddress);

                var request = new Request {Value = "Hello"};


                for (var i = 0; i < 5; i++)
                {
                    var timer = Stopwatch.StartNew();


                    var result = await client.Request(request);

                    timer.Stop();

                    await Console.Out.WriteLineAsync($"Request complete: {timer.ElapsedMilliseconds}ms, Response = {result.ResponseValue}");
                }
            }

            protected override void ConfigureHttpReceiveEndpoint(IHttpReceiveEndpointConfigurator configurator)
            {
                configurator.Consumer<HttpRequestConsumer>();
            }
        }


        public class HttpRequestConsumer :
            IConsumer<Request>
        {
            public async Task Consume(ConsumeContext<Request> context)
            {
                await Console.Out.WriteLineAsync($"Received Request: {context.Message.Value}");

                context.Respond(new Response {RequestValue = context.Message.Value, ResponseValue = $"{context.Message.Value}, World."});
            }
        }


        public class HttpApiRequestConsumer :
            IConsumer<Request>
        {
            public async Task Consume(ConsumeContext<Request> context)
            {
                await Console.Out.WriteLineAsync($"API Received Request: {context.Message.Value}");

                context.Respond(new Response {RequestValue = context.Message.Value, ResponseValue = $"{context.Message.Value}, World. I am your API."});
            }
        }


        public class Request
        {
            public string Value { get; set; }
        }


        public class Response
        {
            public string RequestValue { get; set; }
            public string ResponseValue { get; set; }
        }


        public class HttpMessageEnvelope :
            MessageEnvelope
        {
            public HttpMessageEnvelope(object message, string[] messageTypeNames)
            {
                ConversationId = NewId.NextGuid().ToString();
                MessageId = NewId.NextGuid().ToString();

                MessageType = messageTypeNames;

                Message = message;

                SentTime = DateTime.UtcNow;

                Headers = new Dictionary<string, object>();

                Host = HostMetadataCache.Host;
            }

            public string MessageId { get; set; }
            public string RequestId { get; set; }
            public string CorrelationId { get; set; }
            public string ConversationId { get; set; }
            public string InitiatorId { get; set; }
            public string SourceAddress { get; set; }
            public string DestinationAddress { get; set; }
            public string ResponseAddress { get; set; }
            public string FaultAddress { get; set; }
            public string[] MessageType { get; }
            public object Message { get; }
            public DateTime? ExpirationTime { get; set; }
            public DateTime? SentTime { get; }
            public IDictionary<string, object> Headers { get; }
            public HostInfo Host { get; }
        }
    }
}
