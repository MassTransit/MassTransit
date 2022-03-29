namespace MassTransit.BenchmarkConsole
{
    using System.Threading;
    using System.Threading.Tasks;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Jobs;
    using MediatR;
    using Microsoft.Extensions.DependencyInjection;
    using Util;


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


    [SimpleJob(RuntimeMoniker.Net60)]
    [MemoryDiagnoser]
    public class MediatorBenchmark
    {
        ExampleCommandHandler _handler;
        MassTransit.Mediator.IMediator _mediator;
        IMediator _mediatR;
        IRequestClient<ExampleRequest> _requestClient;

        [GlobalSetup]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddMediatR(typeof(MediatorBenchmark));

            _mediator = Bus.Factory.CreateMediator(cfg =>
            {
                cfg.Consumer<ExampleCommandHandler>();
            });

            var provider = services.BuildServiceProvider();

            _mediatR = provider.GetRequiredService<IMediator>();

            var busControl = Bus.Factory.CreateUsingInMemory(cfg =>
            {
                cfg.ReceiveEndpoint("input-queue", x => x.Consumer<ExampleRequestConsumer>());
            });

            TaskUtil.Await(() => busControl.StartAsync(CancellationToken.None));

            _requestClient = busControl.CreateRequestClient<ExampleRequest>();

            _handler = new ExampleCommandHandler();
        }

        [Benchmark(Description = "Direct")]
        public async Task CallingHandler_Directly()
        {
            var command = new ExampleCommand("Example Arg", 2);
            await _handler.Handle(command, CancellationToken.None);
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
            await _mediator.Send(command, CancellationToken.None);
        }

        [Benchmark(Description = "InMemoryBus")]
        public async Task CallingHandler_WithMassTransitInMemoryBus()
        {
            var request = new ExampleRequest
            {
                Name = "Frank",
                Amount = 123.45m
            };
            await _requestClient.GetResponse<ExampleResponse>(request);
        }
    }


    public class ExampleRequestConsumer :
        IConsumer<ExampleRequest>
    {
        public Task Consume(ConsumeContext<ExampleRequest> context)
        {
            return context.RespondAsync(new ExampleResponse
            {
                Name = context.Message.Name,
                Amount = context.Message.Amount
            });
        }
    }


    public class ExampleRequest
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
    }


    public class ExampleResponse
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
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
