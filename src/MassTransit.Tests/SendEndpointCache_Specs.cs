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
namespace MassTransit.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture, Explicit]
    public class Querying_the_send_endpoint_cache_concurrently :
        InMemoryTestFixture
    {
        [Test]
        public async Task Querying_for_two_endpoints_at_the_same_time()
        {
            await Task.WhenAll(Bus.GetSendEndpoint(new Uri("loopback://localhost/queue_a")),
                Bus.GetSendEndpoint(new Uri("loopback://localhost/queue_b"))).ConfigureAwait(false);

            Assert.DoesNotThrowAsync(async () =>
            {
                await Task.WhenAll(Bus.GetSendEndpoint(new Uri("loopback://localhost/queue_a")),
                    Bus.GetSendEndpoint(new Uri("loopback://localhost/queue_b")));
            });
        }
    }
}