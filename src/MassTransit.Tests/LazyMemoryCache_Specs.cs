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
    using Util.Caching;


    [TestFixture]
    public class LazyMemoryCache_Specs
    {
        [Test, Explicit]
        public async Task Should_store_a_cached_item()
        {
            using (var cache = new LazyMemoryCache<Uri, Data>("endpoints", key => Task.FromResult(new Data {Value = $"The Key: {key}"}),
                x => x.SlidingWindow(TimeSpan.FromSeconds(5))))
            {
                var endpoint = await (await cache.Get(new Uri("loopback://localhost"))).Value;
                Console.WriteLine("Endpoint: {0}", endpoint.Created);

                await Task.Delay(TimeSpan.FromSeconds(2));

                endpoint = await (await cache.Get(new Uri("loopback://localhost"))).Value;
                Console.WriteLine("Endpoint: {0}", endpoint.Created);

                await Task.Delay(TimeSpan.FromSeconds(10));

                endpoint = await (await cache.Get(new Uri("loopback://localhost"))).Value;
                Console.WriteLine("Endpoint: {0}", endpoint.Created);
            }
        }


        interface IData
        {
            string Value { get; }
            DateTime Created { get; }
        }


        class Data :
            IData
        {
            public Data()
            {
                Created = DateTime.Now;
            }

            public string Value { get; set; }
            public DateTime Created { get; set; }
        }
    }
}