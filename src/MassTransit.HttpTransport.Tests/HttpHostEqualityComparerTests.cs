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
    using System.Net.Http;
    using Hosting;
    using NUnit.Framework;


    public class HttpHostEqualityComparerTests
    {
        [Test]
        public void IgnoreReply()
        {
            HttpHostSettings x = new ConfigurationHostSettings("reply","localhost",80, HttpMethod.Post);
            HttpHostSettings y = new ConfigurationHostSettings("http", "localhost", 80, HttpMethod.Post);
            var result = HttpHostEqualityComparer.Default.Equals(x, y);
            Assert.That(result, Is.EqualTo(true));
        }

        [Test]
        public void IgnoreReply_Https()
        {
            HttpHostSettings x = new ConfigurationHostSettings("replys", "localhost", 80, HttpMethod.Post);
            HttpHostSettings y = new ConfigurationHostSettings("https", "localhost", 80, HttpMethod.Post);
            var result = HttpHostEqualityComparer.Default.Equals(x, y);
            Assert.That(result, Is.EqualTo(true));
        }
    }
}