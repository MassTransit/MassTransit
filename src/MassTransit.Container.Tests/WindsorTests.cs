namespace MassTransit.Container.Tests
{
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using NUnit.Framework;

    [TestFixture]
    public class WindsorTests
    {
        [Test]
        public void Bob()
        {
            var c = new WindsorContainer();
            c.Register(
                Component.For<Consumes<object>.All>().ImplementedBy<FakeConsumer>(),
                Component.For<Consumes<object>.All>().ImplementedBy<FakeConsumer2>(),
                Component.For<Consumes<object>.All>().ImplementedBy<FakeConsumer3>()
                );


            

        }
    }
}