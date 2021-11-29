namespace MassTransit.Tests.Middleware
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using JunkDrawer;
    using MassTransit.Middleware;
    using NUnit.Framework;


    [TestFixture]
    public class Layering_retry_components_into_a_set
    {
        [Test]
        [Explicit]
        public async Task Should_support_interaction_between_filters()
        {
            var myFilter = new MyFilter();

            IPipeRouter router = new PipeRouter();

            IPipe<InputContext> pipe = Pipe.New<InputContext>(cfg =>
            {
                cfg.UseConcurrencyLimit(10, router);
                cfg.UseCircuitBreaker(cb =>
                {
                    cb.ActiveThreshold = 5;
                    cb.TrackingPeriod = TimeSpan.FromSeconds(60);
                    cb.TripThreshold = 25;
                    cb.ResetInterval = TimeSpan.FromSeconds(30);

                    cb.Router = router;
                });
                cfg.UseRetry(x => x.Immediate(1));

                cfg.UseFilter(myFilter);
            });

            myFilter.Throw = true;

            router.ConnectPipe(Pipe.New<EventContext<CircuitBreakerOpened>>(x => x.UseFilter(new MyController(router))));


            await Task.WhenAll(Enumerable.Range(0, 140).Select(async index =>
            {
                try
                {
                    await pipe.Send(new InputContext("Hello"));
                }
                catch (Exception ex)
                {
                    await Console.Out.WriteLineAsync($"{DateTime.Now:mm:ss:fff} - Faulted: {ex.Message}");
                }
            }));
        }


        class MyController :
            IFilter<EventContext<CircuitBreakerOpened>>
        {
            readonly IPipeRouter _router;

            public MyController(IPipeRouter router)
            {
                _router = router;
            }

            public async Task Send(EventContext<CircuitBreakerOpened> context, IPipe<EventContext<CircuitBreakerOpened>> next)
            {
                await Console.Out.WriteLineAsync("Changing concurrency limit in response to circuit breaker");

                await _router.SetConcurrencyLimit(1);

                await next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
            }
        }


        class MyFilter :
            IFilter<InputContext>
        {
            public bool Throw { get; set; }

            public async Task Send(InputContext context, IPipe<InputContext> next)
            {
                await Console.Out.WriteLineAsync($"{context.Value} : {DateTime.Now:hh:mm:ss:fff}");

                await Task.Delay(1000);

                if (Throw)
                    throw new IntentionalTestException("MyFilter is throwing");
            }

            public void Probe(ProbeContext context)
            {
            }
        }
    }
}
