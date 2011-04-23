namespace MassTransit.Container.Tests
{
    using System;
    using Magnum.Extensions;
    using NUnit.Framework;

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
}