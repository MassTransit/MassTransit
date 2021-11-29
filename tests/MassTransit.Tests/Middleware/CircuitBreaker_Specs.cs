namespace MassTransit.Tests.Middleware
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using JunkDrawer;
    using MassTransit.Middleware;
    using NUnit.Framework;
    using Util;


    [TestFixture]
    public class Using_a_circuit_breaker
    {
        [Test]
        public async Task Should_allow_the_first_call()
        {
            var router = new PipeRouter();

            var count = 0;
            IPipe<TestContext> pipe = Pipe.New<TestContext>(x =>
            {
                x.UseCircuitBreaker(v =>
                {
                    v.ResetInterval = TimeSpan.FromSeconds(60);
                    v.Router = router;
                });
                x.UseExecute(payload =>
                {
                    Interlocked.Increment(ref count);

                    throw new IntentionalTestException();
                });
            });

            TaskCompletionSource<CircuitBreakerOpened> opened = TaskUtil.GetTask<CircuitBreakerOpened>();
            IPipe<EventContext<CircuitBreakerOpened>> observeCircuitBreaker =
                Pipe.Execute<EventContext<CircuitBreakerOpened>>(x => opened.TrySetResult(x.Event));
            router.ConnectPipe(observeCircuitBreaker);

            var context = new TestContext();

            for (var i = 0; i < 100; i++)
                Assert.That(async () => await pipe.Send(context).ConfigureAwait(false), Throws.TypeOf<IntentionalTestException>());

            Assert.That(count, Is.EqualTo(6));

            await opened.Task;
        }


        class TestContext :
            BasePipeContext,
            PipeContext
        {
        }
    }
}
