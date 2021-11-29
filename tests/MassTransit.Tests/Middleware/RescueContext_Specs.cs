namespace MassTransit.Tests.Middleware
{
    namespace Rescues
    {
        using System;
        using System.Threading;
        using System.Threading.Tasks;
        using MassTransit.Middleware;
        using NUnit.Framework;


        [TestFixture]
        public class RescueContext_Specs
        {
            [Test]
            public async Task Should_invoke_the_rescue_pipe()
            {
                var count = 0;
                IPipe<SomeContext> pipe = Pipe.New<SomeContext>(x =>
                {
                    x.UseRescue((cxt, ex) => (SomeRescueContext)new SomeRescueContextImpl(cxt, ex), r =>
                    {
                        r.UseExecute(c => Interlocked.Increment(ref count));

                        r.Handle<IntentionalTestException>();
                    });

                    x.UseExecute(cxt =>
                    {
                        throw new IntentionalTestException("Kaboom!");
                    });
                });

                var context = new SomeContextImpl();

                await pipe.Send(context);

                Assert.That(count, Is.EqualTo(1));
            }
        }


        interface SomeContext :
            PipeContext
        {
        }


        class SomeContextImpl :
            BasePipeContext,
            SomeContext
        {
        }


        interface SomeRescueContext :
            SomeContext
        {
            Exception Exception { get; }
        }


        class SomeRescueContextImpl :
            ProxyPipeContext,
            SomeRescueContext
        {
            public SomeRescueContextImpl(SomeContext context, Exception exception)
                : base(context)
            {
                Exception = exception;
            }

            public Exception Exception { get; }
        }
    }
}
