namespace MassTransit.BenchmarkConsole
{
    using System.Threading;
    using System.Threading.Tasks;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Jobs;
    using MediatR;
    using Microsoft.Extensions.DependencyInjection;


    public class ExampleCommand :
        IRequest
    {
        public ExampleCommand(string arg1, int arg2)
        {
            Arg1 = arg1;
            Arg2 = arg2;
        }

        public string Arg1 { get; }

        public int Arg2 { get; }
    }


    [SimpleJob(RuntimeMoniker.Net50)]
    [MemoryDiagnoser]
    public class MediatorBenchmark
    {
        ExampleCommandHandler _handler;
        MassTransit.Mediator.IMediator _mediator;
        IMediator _mediatR;

        [GlobalSetup]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddMediatR(typeof(MediatorBenchmark));
            services.AddMediator(x =>
            {
                x.AddConsumer<ExampleCommandHandler>();
            });

            var provider = services.BuildServiceProvider();
            _mediatR = provider.GetRequiredService<IMediator>();
            _mediator = provider.GetRequiredService<MassTransit.Mediator.IMediator>();
            _handler = new ExampleCommandHandler();
        }

        [Benchmark(Description = "MediatR")]
        public async Task CallingHandler_WithMediator()
        {
            var command = new ExampleCommand("Example Arg", 2);
            await _mediatR.Send(command, CancellationToken.None);
        }

        [Benchmark(Description = "MassTransit")]
        public async Task CallingHandler_WithMassTransitMediator()
        {
            var command = new ExampleCommand("Example Arg", 2);
            await _mediatR.Send(command, CancellationToken.None);
        }

        [Benchmark(Description = "Direct")]
        public async Task CallingHandler_Directly()
        {
            var command = new ExampleCommand("Example Arg", 2);
            await _handler.Handle(command, CancellationToken.None);
        }

        [Benchmark(Description = "Transient")]
        public async Task CallingHandler_Directly_Transient()
        {
            var handler = new ExampleCommandHandler();
            var command = new ExampleCommand("Example Arg", 2);
            await handler.Handle(command, CancellationToken.None);
        }
    }


    public class ExampleCommandHandler :
        IRequestHandler<ExampleCommand>,
        IConsumer<ExampleCommand>
    {
        public Task Consume(ConsumeContext<ExampleCommand> context)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<Unit> Handle(ExampleCommand request, CancellationToken cancellationToken)
        {
            return Unit.Task;
        }
    }
}
