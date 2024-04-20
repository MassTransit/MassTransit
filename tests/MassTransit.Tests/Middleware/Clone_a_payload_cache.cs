namespace MassTransit.Tests.Middleware
{
    using MassTransit.Middleware;
    using NUnit.Framework;


    [TestFixture]
    public class Clone_a_payload_cache
    {
        [Test]
        public void Scoped_context_should_not_update_parent()
        {
            var p = new TestPipeContext();
            p.GetOrAddPayload(() => new Item("bob"));

            Item i;
            Assert.That(p.TryGetPayload(out i), Is.True);

            var p2 = new TestProxyPipeContext(p);

            p2.GetOrAddPayload(() => new Item2("bill"));

            Assert.Multiple(() =>
            {
                Assert.That(p.TryGetPayload(out Item2 _), Is.False);
                Assert.That(p2.TryGetPayload(out Item2 _), Is.True);
            });
            Assert.That(p2.TryGetPayload(out Item _), Is.True);
        }


        class TestPipeContext :
            BasePipeContext,
            PipeContext
        {
        }


        class TestProxyPipeContext :
            ScopePipeContext,
            PipeContext
        {
            public TestProxyPipeContext(PipeContext parentContext)
                : base(parentContext)
            {
            }
        }


        class Item
        {
            public Item(string name)
            {
                Name = name;
            }

            public string Name { get; }
        }


        class Item2
        {
            public Item2(string name)
            {
                Name = name;
            }

            public string Name { get; }
        }
    }
}
