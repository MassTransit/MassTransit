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
namespace MassTransit.Containers.Tests
{
    using System.Threading.Tasks;
    using Autofac;
    using AutofacIntegration;
    using NUnit.Framework;


    [TestFixture]
    public class AutofacContainer_RegistrationExtension
    {
        [Test]
        public void Registration_extension_method_for_consumers()
        {
            var builder = new ContainerBuilder();

            builder.RegisterConsumers(System.Reflection.Assembly.GetExecutingAssembly());

            var container = builder.Build();

            Assert.That(container.IsRegistered<TestConsumer>(), Is.True);
        }
    }

    public class TestConsumer : IConsumer
    { }
}