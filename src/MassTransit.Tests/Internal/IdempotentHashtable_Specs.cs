namespace MassTransit.Tests.Internal
{
    using System;
    using MassTransit.Internal;
    using NUnit.Framework;

    public class IdempotentHashtable_Specs : 
        Specification
    {
        Uri a = new Uri("msmq://localhost/bob");
        Uri b = new Uri("msmq://localhost/bill");

        [Test]
        public void Adds()
        {
            

            IdempotentHashtable<Uri, object> table = new IdempotentHashtable<Uri, object>();
           
            table.Add(a, new object());
            table.Add(a, new object());

            Assert.AreEqual(1, table.Count);

            table.Add(b, new object());
            Assert.AreEqual(2, table.Count);
        }

        [Test]
        public void Removes()
        {
            IdempotentHashtable<Uri, object> table = new IdempotentHashtable<Uri, object>();

            table.Add(a, new object());
            table.Add(b, new object());

            table.Remove(a);
            table.Remove(a);
            table.Remove(a);
            table.Remove(a);

            Assert.AreEqual(1, table.Count);
        }
    }
}