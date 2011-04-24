// Copyright 2007-2011 The Apache Software Foundation.
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
namespace MassTransit.Container.Tests
{
    using System;
    using System.Linq;
    using Ninject;
    using Ninject.Activation;
    using NUnit.Framework;

    [TestFixture]
    public class NinjectTests
    {
        [Test][Ignore]
        public void Bob()
        {
            var k = new StandardKernel();
            k.Bind<IConsumer>().To<FakeConsumer>();
            k.Bind<IConsumer>().To<FakeConsumer2>();

            foreach (var binding in k.GetBindings(typeof(IConsumer)))
            {
                var cb = binding.ProviderCallback;
                var req = new Request(null, typeof(IConsumer), null, () => null);
                var x = cb(new Context(k, req, null, null, null, null));
                var concrete = x.Type;

                Console.WriteLine(concrete);
            }

           
            int i = 0;
        }
    }


    public class FakeConsumer : Consumes<object>.All
    {
        public void Consume(object message)
        {
        }
    }

    public class FakeConsumer2 : FakeConsumer
    {
    }
    public class FakeConsumer3 : FakeConsumer{}
}