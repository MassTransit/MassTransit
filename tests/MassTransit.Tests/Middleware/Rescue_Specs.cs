namespace MassTransit.Tests.Middleware
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Middleware;
    using NUnit.Framework;


    [TestFixture]
    public class Using_the_rescue_filter
    {
        [Test]
        public async Task Should_invoke_the_rescue_pipe()
        {
            var count = 0;
            IPipe<ITestContext> pipe = Pipe.New<ITestContext>(x =>
            {
                x.UseRescue(Pipe.New<TestExceptionContext>(r =>
                {
                    r.UseExecute(c => Interlocked.Increment(ref count));
                }), (cxt, ex) => new TestExceptionContext(cxt, ex), r => r.Handle<IntentionalTestException>());

                x.UseExecute(cxt =>
                {
                    throw new IntentionalTestException("Kaboom!");
                });
            });

            var context = new TestContext();

            await pipe.Send(context);

            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public async Task Should_skip_if_filtered_exception()
        {
            var count = 0;
            IPipe<ITestContext> pipe = Pipe.New<ITestContext>(x =>
            {
                x.UseRescue(Pipe.New<TestExceptionContext>(r =>
                {
                    r.UseExecute(c => Interlocked.Increment(ref count));
                }), (cxt, ex) => new TestExceptionContext(cxt, ex), r => r.Ignore<IntentionalTestException>());

                x.UseExecute(cxt =>
                {
                    throw new IntentionalTestException("Kaboom!");
                });
            });

            var context = new TestContext();

            Assert.That(async () => await pipe.Send(context), Throws.TypeOf<IntentionalTestException>());
        }

        [Test]
        public async Task Should_support_an_aggregate_exception()
        {
            var count = 0;
            IPipe<ITestContext> pipe = Pipe.New<ITestContext>(x =>
            {
                x.UseRescue(Pipe.New<TestExceptionContext>(r =>
                {
                    r.UseExecute(c => Interlocked.Increment(ref count));
                }), (cxt, ex) => new TestExceptionContext(cxt, ex));

                x.UseExecute(cxt =>
                {
                    throw new AggregateException("Boom!");
                });
            });

            var context = new TestContext();

            await pipe.Send(context);

            Assert.That(count, Is.EqualTo(1));
        }


        interface ITestContext :
            PipeContext
        {
        }


        class TestContext :
            BasePipeContext,
            ITestContext
        {
        }


        class TestExceptionContext :
            ProxyPipeContext,
            ITestContext
        {
            public TestExceptionContext(ITestContext context, Exception exception)
                : base(context)
            {
                Exception = exception;
            }

            public Exception Exception { get; }
        }
    }
}
