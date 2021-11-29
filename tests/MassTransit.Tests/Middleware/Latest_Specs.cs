namespace MassTransit.Tests.Middleware
{
    using System.Threading.Tasks;
    using MassTransit.Middleware;
    using NUnit.Framework;


    [TestFixture]
    public class Using_the_latest_filter_on_the_pipe
    {
        [Test]
        public async Task Should_keep_track_of_only_the_last_value()
        {
            ILatestFilter<IInputContext<A>> latestFilter = null;

            IPipe<IInputContext<A>> pipe = Pipe.New<IInputContext<A>>(x =>
            {
                x.UseLatest(l => l.Created = filter => latestFilter = filter);
                x.UseExecute(payload =>
                {
                });
            });

            Assert.That(latestFilter, Is.Not.Null);

            var inputContext = new InputContext(new object());

            var limit = 100;
            for (var i = 0; i <= limit; i++)
            {
                var context = new InputContext<A>(inputContext, new A { Index = i });
                await pipe.Send(context);
            }

            IInputContext<A> latest = await latestFilter.Latest;

            Assert.That(latest.Value.Index, Is.EqualTo(limit));
        }


        class A
        {
            public int Index { get; set; }
        }
    }
}
