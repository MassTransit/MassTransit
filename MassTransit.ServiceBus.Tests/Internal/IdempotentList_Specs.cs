namespace MassTransit.ServiceBus.Tests.Internal
{
    using System;
    using MassTransit.ServiceBus.Internal;
    using NUnit.Framework;

    public class IdempotentList_Specs :
        Specification
    {
        Uri a = new Uri("msmq://localhost/test");
        Uri b = new Uri("msmq://localhost/test");
        Uri c = new Uri("msmq://localhost/test2");

        [Test]
        public void Add()
        {
            IdempotentList<Uri> list = new IdempotentList<Uri>();
            list.Add(a);
            list.Add(b);

            Assert.AreEqual(1, list.Count);

            list.Add(c);
            Assert.AreEqual(2, list.Count);
        }

        [Test]
        public void Remove()
        {
            IdempotentList<Uri> list = new IdempotentList<Uri>();
            list.Add(a);
            list.Add(b);
            list.Add(c);
            list.Remove(a);
            Assert.AreEqual(1, list.Count);

            
            
        }
    }
}