#nullable enable
namespace MassTransit.BenchmarkConsole
{
    using System;
    using System.Threading.Tasks;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Jobs;
    using Contracts;
    using Middleware;
    using Throughput;


    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [SimpleJob(RuntimeMoniker.Net50)]
    [MemoryDiagnoser]
    public class SendBenchmark
    {
        readonly SetConcurrencyLimit _command = new Command(32);
        readonly IPipe<TestContext> _concurrencyPipe;
        readonly TestContext _context;
        readonly IPipe<PipeContext> _dispatchPipe;
        readonly IPipe<PipeContext> _doubleDispatchPipe;
        readonly IPipe<TestContext> _doublePipe;
        readonly IPipe<TestContext> _emptyPipe;
        readonly IPipe<TestContext> _faultPipe;
        readonly IPipe<TestContext> _retryPipe;
        readonly IPipe<PipeContext> _tripleDispatchPipe;

        public SendBenchmark()
        {
            _context = new ThroughputTestContext(Guid.NewGuid(), "Payload");

            _emptyPipe = Pipe.Empty<TestContext>();

            _retryPipe = Pipe.New<TestContext>(x =>
            {
                x.UseRetry(r => r.Immediate(1));

                x.UseFilter(new BenchmarkFilter());
            });

            _concurrencyPipe = Pipe.New<TestContext>(x =>
            {
                x.UseConcurrencyLimit(Environment.ProcessorCount);

                x.UseFilter(new BenchmarkFilter());
            });

            _doublePipe = Pipe.New<TestContext>(x =>
            {
                x.UseConcurrencyLimit(Environment.ProcessorCount);
                x.UseRetry(r => r.Immediate(1));

                x.UseFilter(new BenchmarkFilter());
            });

            _faultPipe = Pipe.New<TestContext>(x =>
            {
                x.UseRetry(r => r.Immediate(1));

                x.UseFilter(new FaultFilter());
            });

            var dispatchPipe = new PipeRouter();
            _dispatchPipe = dispatchPipe;

            dispatchPipe.ConnectPipe(Pipe.Empty<CommandContext<SetConcurrencyLimit>>());

            var doubleDispatchPipe = new PipeRouter();
            _doubleDispatchPipe = doubleDispatchPipe;

            doubleDispatchPipe.ConnectPipe(Pipe.Empty<CommandContext<SetConcurrencyLimit>>());
            doubleDispatchPipe.ConnectPipe(Pipe.Empty<CommandContext<SetRateLimit>>());

            var tripleDispatchPipe = new PipeRouter();
            _tripleDispatchPipe = tripleDispatchPipe;

            tripleDispatchPipe.ConnectPipe(Pipe.Empty<CommandContext<SetConcurrencyLimit>>());
            tripleDispatchPipe.ConnectPipe(Pipe.Empty<CommandContext<SetRateLimit>>());
            tripleDispatchPipe.ConnectPipe(Pipe.Empty<CommandContext<CircuitBreakerOpened>>());
        }

        [Benchmark]
        public async Task EmptyPipe()
        {
            await _emptyPipe.Send(_context);
        }

        [Benchmark]
        public async Task RetryPipe()
        {
            await _retryPipe.Send(_context);
        }

        [Benchmark]
        public async Task ConcurrencyPipe()
        {
            await _concurrencyPipe.Send(_context);
        }

        [Benchmark]
        public async Task DoublePipe()
        {
            await _doublePipe.Send(_context);
        }

        [Benchmark]
        public async Task DispatchPipe()
        {
            await _dispatchPipe.SendCommand(_command);
        }

        [Benchmark]
        public async Task DoubleDispatchPipe()
        {
            await _doubleDispatchPipe.SendCommand(_command);
        }

        [Benchmark]
        public async Task TripleDispatchPipe()
        {
            await _tripleDispatchPipe.SendCommand(_command);
        }

        public async Task FaultPipe()
        {
            try
            {
                await _faultPipe.Send(_context);
            }
            catch
            {
            }
        }


        class Command :
            SetConcurrencyLimit
        {
            public Command(int concurrencyLimit)
            {
                ConcurrencyLimit = concurrencyLimit;
            }

            public DateTime? Timestamp { get; } = DateTime.UtcNow;
            public string? Id => null;
            public int ConcurrencyLimit { get; }
        }
    }
}
