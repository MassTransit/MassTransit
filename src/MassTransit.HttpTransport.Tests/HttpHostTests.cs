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
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Clients;
    using Configuration.Builders;
    using GreenPipes;
    using Hosting;
    using MassTransit.Pipeline.Pipes;
    using NUnit.Framework;
    using Serialization;
    using Transports;
    using Util;


    public class HttpHostTests
    {
        [Test]
        public async Task X()
        {
            var sup = new TaskSupervisor("test");
            var cache = new OwinHostCache(new ConfigurationHostSettings("http", "localhost", 8080, HttpMethod.Post), sup);
            await cache.Send(Pipe.Empty<OwinHostContext>(), default(CancellationToken));
            await sup.Stop("test");
        }

        [Test]
        public async Task HttpHost_NoEndpoints()
        {
            var host = new HttpHost(new ConfigurationHostSettings("http","localhost",8080, HttpMethod.Post));
            var handle = await host.Start();
            using (var tokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(1)))
            {
                await handle.Stop(tokenSource.Token);
            }
        }

        [Test]
        public async Task HttpHost_Endpoints()
        {
            var host = new HttpHost(new ConfigurationHostSettings("http", "localhost", 8080, HttpMethod.Post));
            var hosts = new BusHostCollection<HttpHost>();
            hosts.Add(host);

            var ser = new JsonMessageSerializer();
            var stp = new HttpSendTransportProvider(hosts);
            SendPipe sp = null;
            var sep = new HttpSendEndpointProvider(ser, new Uri("http://localhost:8080"), stp, sp);
            var pep = new HttpPublishEndpointProvider(null);
            var httpReceiveTransport = new HttpReceiveTransport(host, sep, pep, ser, null);
            var receiveTransportHandle = httpReceiveTransport.Start(Pipe.Empty<ReceiveContext>());

            var handle = await host.Start();
            using (var tokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(1)))
            {
                await receiveTransportHandle.Stop(tokenSource.Token);
                await handle.Stop(tokenSource.Token);
            }
        }

        [Test]
        public void Endpoint()
        {
            
        }
    }
}