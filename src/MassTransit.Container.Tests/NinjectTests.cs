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
    using NUnit.Framework;
    using Magnum.Extensions;

    [TestFixture]
    public class NinjectTests
    {
        [Test]
        public void Bob()
        {
            var k = new StandardKernel();
            k.Bind<Consumes<object>.All>().To<FakeConsumer>();
            k.Bind<Consumes<object>.All>().To<FakeConsumer2>();

            foreach (var binding in k.GetAll<Consumes<object>.All>())
            {
                Console.WriteLine("Hi");
            }

            
            int i = 0;
        }
    }

    [TestFixture]
    public class StructureMapTests
    {
        [Test]
        public void Test()
        {
            var c = new StructureMap.Container(cfg=>
            {
                cfg.For<Consumes<object>.All>().Use<FakeConsumer>();
                cfg.For<Consumes<object>.All>().Use<FakeConsumer2>();
                cfg.For<Consumes<object>.All>().Use<FakeConsumer3>();
            });

            var i = c.Model.AllInstances;

            foreach (var instanceRef in i)
            {
                var pt = instanceRef.PluginType;
                if(pt.Implements(typeof(IConsumer)))
                    Console.WriteLine(pt);
            }

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