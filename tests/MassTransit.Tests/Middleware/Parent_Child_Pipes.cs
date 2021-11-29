namespace MassTransit.Tests.Middleware
{
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Middleware;
    using NUnit.Framework;


    [TestFixture]
    public class Parent_Child_Pipes
    {
        [Test]
        public async Task Should_compose_pipes()
        {
            var count1 = 0;
            var count2 = 0;

            IPipe<InitialContext> pipe1 = Pipe.New<InitialContext>(cfg =>
            {
                cfg.UseExecute(cxt =>
                {
                    Interlocked.Increment(ref count1);
                });
            });

            IPipe<InitialContext> pipe2 = Pipe.New<InitialContext>(cfg =>
            {
                cfg.UseFork(pipe1);

                cfg.UseExecuteAsync(async cxt =>
                {
                    Interlocked.Increment(ref count2);
                });
            });

            await pipe2.Send(new InitialContext()).ConfigureAwait(false);

            Assert.That(count1, Is.EqualTo(1));
            Assert.That(count2, Is.EqualTo(1));
        }

        [Test]
        public async Task ShouldDeliverToBoth()
        {
            var count1 = 0;
            var count2 = 0;

            IPipe<InitialContext> pipe2 = Pipe.New<InitialContext>(cfg =>
            {
                cfg.UseExecuteAsync(async cxt =>
                {
                    IPipe<SubContext> pipe1 = Pipe.New<SubContext>(subCfg =>
                    {
                        subCfg.UseExecute(subCxt =>
                        {
                            Interlocked.Increment(ref count1);
                        });
                    });


                    await pipe1.Send(new SubContext()).ConfigureAwait(false);

                    Interlocked.Increment(ref count2);
                });
            });


            await pipe2.Send(new InitialContext()).ConfigureAwait(false);

            Assert.That(count1, Is.EqualTo(1));
            Assert.That(count2, Is.EqualTo(1));
        }


        class InitialContext :
            BasePipeContext,
            PipeContext
        {
        }


        class SubContext :
            BasePipeContext,
            PipeContext
        {
        }
    }
}
