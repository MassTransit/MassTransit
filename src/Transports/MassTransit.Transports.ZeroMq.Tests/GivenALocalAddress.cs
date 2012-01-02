// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports.ZeroMq.Tests
{
    using System;
    using Magnum.TestFramework;
    using NUnit.Framework;

    [TestFixture]
    public class GivenALocalAddress
    {
        public Uri Uri = new Uri("zeromq-pgm://some_server:1234/");
        ZeroMqAddress _addr;

        [When]
        public void WhenParsed()
        {
            _addr = ZeroMqAddress.Parse(Uri);
        }

        [Then]
        public void TheHost()
        {
            _addr.Host.ShouldEqual("some_server");
        }

        [Then]
        public void ThePort()
        {
            _addr.Port.ShouldEqual(1234);
        }

        [Then]
        public void IsLocal()
        {
            _addr.IsLocal.ShouldBeTrue();
        }

        [Then]
        public void IsTransactional()
        {
            _addr.IsTransactional.ShouldBeFalse();
        }

        [Then]
        public void Rebuilt()
        {
            _addr.RebuiltUri.ShouldEqual(new Uri("pgm://some_server:1234"));
        }
    }
}