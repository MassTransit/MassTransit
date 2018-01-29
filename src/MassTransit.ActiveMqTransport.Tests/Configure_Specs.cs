// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.ActiveMqTransport.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class Configuring_ActiveMQ
    {
        [Test]
        public async Task Should_succeed_and_connect_when_properly_configured()
        {
            var busControl = Bus.Factory.CreateUsingActiveMq(cfg =>
            {
                cfg.Host("b-15a8b984-a883-4143-a4e7-8f97bc5db37d-1.mq.us-east-2.amazonaws.com", 61617, h =>
                {
                    h.Username("masstransit-build");
                    h.Password("build-Br0k3r");

                    h.UseSsl();
                });
            });

            await busControl.StartAsync();

            await busControl.StopAsync();
        }

        [Test]
        public void Should_succeed_when_properly_configured()
        {
            var busControl = Bus.Factory.CreateUsingActiveMq(cfg =>
            {
                cfg.Host("b-15a8b984-a883-4143-a4e7-8f97bc5db37d-1.mq.us-east-2.amazonaws.com", 61617, h =>
                {
                    h.Username("masstransit-build");
                    h.Password("build-Br0k3r");

                    h.UseSsl();
                });
            });
        }
    }
}