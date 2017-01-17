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
namespace MassTransit.Tests.Transports
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Transports.InMemory;
    using MassTransit.Transports.InMemory.Fabric;
    using NUnit.Framework;


    [TestFixture]
    public class MessageFabric_Specs
    {
        [Test]
        public async Task Should_allow_a_legitimate_binding()
        {
            var fabric = new MessageFabric(16);

             fabric.ExchangeBind("Namespace.A", "input-exchange");
             fabric.ExchangeBind("Namespace.B", "input-exchange");
             fabric.QueueBind("input-exchange", "input-queue");
        }

        [Test]
        public async Task Should_not_allow_a_cyclic_binding()
        {
            var fabric = new MessageFabric(16);

             fabric.ExchangeBind("Namespace.A", "input-exchange");
             fabric.ExchangeBind("Namespace.B", "input-exchange");

             fabric.ExchangeBind("input-exchange", "output-exchange");

            Assert.That(() =>  fabric.ExchangeBind("output-exchange", "Namespace.A"), Throws.TypeOf<InvalidOperationException>());
        }
    }
}