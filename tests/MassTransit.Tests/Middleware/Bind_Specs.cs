namespace MassTransit.Tests.Middleware
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Internals;
    using MassTransit.Middleware;
    using NUnit.Framework;
    using Util;


    [TestFixture]
    public class Bind_Specs
    {
        [Test]
        public async Task Should_include_the_bound_context_in_the_pipe()
        {
            Filter filter = null;
            IPipe<InputContext> pipe = Pipe.New<InputContext>(cfg =>
            {
                cfg.UseBind(x => x.Source(new ThingFactory(), p =>
                {
                    p.ContextPipe.UseExecuteAsync(context => Console.Out.WriteLineAsync($"ContextPipe: {context.Value}"));
                    filter = new Filter();
                    p.UseFilter(filter);
                }));
            });

            await pipe.Send(new InputContext("Input"));

            await filter.GotTheThing.OrTimeout(s: 5);
        }


        class Filter :
            IFilter<BindContext<InputContext, Thing>>
        {
            readonly TaskCompletionSource<Thing> _completed;

            public Filter()
            {
                _completed = TaskUtil.GetTask<Thing>();
            }

            public Task<Thing> GotTheThing => _completed.Task;

            public async Task Send(BindContext<InputContext, Thing> context, IPipe<BindContext<InputContext, Thing>> next)
            {
                _completed.SetResult(context.Right);

                await next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
            }
        }


        class ThingFactory :
            IPipeContextSource<Thing, InputContext>
        {
            public Task Send(InputContext context, IPipe<Thing> pipe)
            {
                var thing = new Thing { Value = "Rock!" };

                return pipe.Send(thing);
            }

            public void Probe(ProbeContext context)
            {
            }
        }


        class Thing :
            BasePipeContext,
            PipeContext
        {
            public string Value { get; set; }
        }
    }
}
