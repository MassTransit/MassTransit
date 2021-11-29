namespace MassTransit.Tests.Middleware
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class OneTime_Specs
    {
        [Test]
        public async Task Should_only_invoke_the_method_once()
        {
            var callCount = 0;
            var totalCount = 0;

            IPipe<IInputContext> pipe = Pipe.New<IInputContext>(cfg =>
            {
                cfg.UseExecuteAsync(async x =>
                {
                    await x.OneTimeSetup<Secret<IInputContext>>(async key =>
                    {
                        Interlocked.Increment(ref callCount);
                    }, () => new MySecret<IInputContext>());

                    Interlocked.Increment(ref totalCount);
                });
            });

            var context = new InputContext("Hello");

            Task[] tasks = Enumerable.Range(0, 50)
                .Select(index => Task.Run(async () => await pipe.Send(context)))
                .ToArray();

            await Task.WhenAll(tasks);

            Assert.That(callCount, Is.EqualTo(1));
            Assert.That(totalCount, Is.EqualTo(50));
        }


        public interface Secret<T>
        {
        }


        public class MySecret<T> :
            Secret<T>
        {
        }
    }
}
