// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests.NewId_
{
    using NUnit.Framework;
    using NewIdProviders;


    [TestFixture]
    public class When_getting_a_network_address_for_the_id_generator
    {
        [Test]
        public void Should_pull_the_network_adapter_mac_address()
        {
            var networkIdProvider = new NetworkAddressWorkerIdProvider();

            byte[] networkId = networkIdProvider.GetWorkerId(0);

            Assert.IsNotNull(networkId);
            Assert.AreEqual(6, networkId.Length);
        }
    }
}