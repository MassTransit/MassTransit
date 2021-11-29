namespace MassTransit.Tests.Middleware
{
    using System;
    using System.Threading;
    using MassTransit.Middleware;
    using NUnit.Framework;


    [TestFixture]
    public class RetryDifferent_Specs
    {
        [Test]
        public void Should_retry_the_specified_times_and_fail()
        {
            var count = 0;
            IPipe<TestContext> pipe = Pipe.New<TestContext>(x =>
            {
                x.UseRetry(r =>
                {
                    r.Handle<IntentionalTestException>();
                    r.Immediate(10);
                });

                x.UseRetry(r =>
                {
                    r.Handle<InvalidOperationException>();
                    r.Immediate(5);
                });

                x.UseExecute(payload =>
                {
                    var current = Interlocked.Increment(ref count);
                    if (current % 2 == 0)
                        throw new IntentionalTestException("Kaboom!");

                    throw new InvalidOperationException("Expected, but unwarranted");
                });
            });

            var context = new TestContext();

            Assert.That(async () => await pipe.Send(context), Throws.TypeOf<IntentionalTestException>());

            Assert.That(count, Is.EqualTo(22));
        }


        class TestContext :
            BasePipeContext,
            PipeContext
        {
        }
    }
}
