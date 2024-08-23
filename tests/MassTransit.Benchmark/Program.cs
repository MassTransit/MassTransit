namespace MassTransitBenchmark
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using BusOutbox;
    using Latency;
    using MassTransit.Logging;
    using MassTransit.Monitoring;
    using NDesk.Options;
    using OpenTelemetry;
    using OpenTelemetry.Metrics;
    using OpenTelemetry.Resources;
    using OpenTelemetry.Trace;
    using RequestResponse;


    class Program
    {
        static List<string> _remaining;

        static async Task Main(string[] args)
        {
            Console.WriteLine("MassTransit Benchmark");
            Console.WriteLine();

            var optionSet = new ProgramOptionSet();

            var disposables = new List<IDisposable>();
            try
            {
                _remaining = optionSet.Parse(args);

                if (optionSet.Help)
                {
                    ShowHelp(optionSet);
                    return;
                }

                if (optionSet.Verbose)
                {
                }

                if (optionSet.EnableMetrics)
                {
                    disposables.Add(Sdk.CreateMeterProviderBuilder()
                        .AddMeter(InstrumentationOptions.MeterName)
                        .ConfigureResource(r => r.AddService("MassTransit.Benchmark"))
                        .AddOtlpExporter()
                        .Build());
                }

                if (optionSet.EnableTraces)
                {
                    disposables.Add(Sdk.CreateTracerProviderBuilder()
                        .AddSource(DiagnosticHeaders.DefaultListenerName)
                        .ConfigureResource(r => r.AddService("MassTransit.Benchmark"))
                        .AddOtlpExporter()
                        .Build());
                }

                optionSet.ShowOptions();

                if (optionSet.Threads.HasValue)
                {
                    ThreadPool.GetMinThreads(out var workerThreads, out var completionPortThreads);
                    ThreadPool.SetMinThreads(Math.Max(workerThreads, optionSet.Threads.Value), completionPortThreads);
                }

                if (optionSet.Benchmark.HasFlag(ProgramOptionSet.BenchmarkOptions.Latency))
                    await RunLatencyBenchmark(optionSet);

                if (optionSet.Benchmark.HasFlag(ProgramOptionSet.BenchmarkOptions.Rpc))
                    RunRequestResponseBenchmark(optionSet);

                if (optionSet.Benchmark.HasFlag(ProgramOptionSet.BenchmarkOptions.BusOutbox))
                    await Task.Run(() => RunBusOutboxBenchmark(optionSet));

                if (Debugger.IsAttached)
                {
                    Console.Write("Press any key to continue...");
                    Console.ReadKey();
                }
            }
            catch (OptionException ex)
            {
                Console.Write("mtbench: ");
                Console.WriteLine(ex.Message);
                Console.WriteLine("Use 'mtbench --help' for detailed usage information.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Crashed: {0}", ex.Message);
            }
            finally
            {
                disposables.ForEach(x => x.Dispose());
            }
        }

        static async Task RunLatencyBenchmark(ProgramOptionSet optionSet)
        {
            var messageLatencyOptionSet = new MessageLatencyOptionSet();

            messageLatencyOptionSet.Parse(_remaining);

            IMessageLatencySettings settings = messageLatencyOptionSet;

            IMessageLatencyTransport transport;
            if (optionSet.Transport == ProgramOptionSet.TransportOptions.AzureServiceBus)
            {
                var serviceBusOptionSet = new ServiceBusOptionSet();

                serviceBusOptionSet.Parse(_remaining);

                serviceBusOptionSet.ShowOptions();

                ServicePointManager.Expect100Continue = false;
                ServicePointManager.UseNagleAlgorithm = false;

                transport = new ServiceBusMessageLatencyTransport(serviceBusOptionSet, settings);
            }
            else if (optionSet.Transport == ProgramOptionSet.TransportOptions.RabbitMq)
            {
                var rabbitMqOptionSet = new RabbitMqOptionSet();
                rabbitMqOptionSet.Parse(_remaining);

                rabbitMqOptionSet.ShowOptions();

                transport = new RabbitMqMessageLatencyTransport(rabbitMqOptionSet, settings);
            }
            else if (optionSet.Transport == ProgramOptionSet.TransportOptions.AmazonSqs)
            {
                var amazonSqsOptionSet = new AmazonSqsOptionSet();
                amazonSqsOptionSet.Parse(_remaining);

                amazonSqsOptionSet.ShowOptions();

                transport = new AmazonSqsMessageLatencyTransport(amazonSqsOptionSet, settings);
            }
            else if (optionSet.Transport == ProgramOptionSet.TransportOptions.ActiveMq)
            {
                var activeMqOptionSet = new ActiveMqOptionSet();
                activeMqOptionSet.Parse(_remaining);

                activeMqOptionSet.ShowOptions();

                transport = new ActiveMqMessageLatencyTransport(activeMqOptionSet, settings);
            }
            else if (optionSet.Transport == ProgramOptionSet.TransportOptions.Kafka)
            {
                var kafkaOptionSet = new KafkaOptionSet();
                kafkaOptionSet.Parse(_remaining);

                kafkaOptionSet.ShowOptions();

                transport = new KafkaMessageLatencyTransport(kafkaOptionSet, settings);
            }
            else if (optionSet.Transport == ProgramOptionSet.TransportOptions.Sql)
            {
                var options = new SqlOptionSet();
                options.Parse(_remaining);

                options.ShowOptions();

                transport = new SqlMessageLatencyTransport(options, settings);
            }
            else if (optionSet.Transport == ProgramOptionSet.TransportOptions.Mediator)
                transport = new MediatorMessageLatencyTransport(settings);
            else
            {
                var inMemoryOptionSet = new InMemoryOptionSet();
                inMemoryOptionSet.Parse(_remaining);

                inMemoryOptionSet.ShowOptions();

                transport = new InMemoryMessageLatencyTransport(inMemoryOptionSet, settings);
            }

            var benchmark = new MessageLatencyBenchmark(transport, settings);

            await benchmark.Run();
        }

        static void RunRequestResponseBenchmark(ProgramOptionSet optionSet)
        {
            var requestResponseOptionSet = new RequestResponseOptionSet();

            requestResponseOptionSet.Parse(_remaining);

            IRequestResponseSettings settings = requestResponseOptionSet;

            IRequestResponseTransport transport;
            if (optionSet.Transport == ProgramOptionSet.TransportOptions.AzureServiceBus)
            {
                var serviceBusOptionSet = new ServiceBusOptionSet();

                serviceBusOptionSet.Parse(_remaining);

                serviceBusOptionSet.ShowOptions();

                ServicePointManager.Expect100Continue = false;
                ServicePointManager.UseNagleAlgorithm = false;

                transport = new ServiceBusRequestResponseTransport(serviceBusOptionSet, settings);
            }
            else if (optionSet.Transport == ProgramOptionSet.TransportOptions.RabbitMq)
            {
                var rabbitMqOptionSet = new RabbitMqOptionSet();
                rabbitMqOptionSet.Parse(_remaining);

                rabbitMqOptionSet.ShowOptions();

                transport = new RabbitMqRequestResponseTransport(rabbitMqOptionSet, settings);
            }
            else if (optionSet.Transport == ProgramOptionSet.TransportOptions.Mediator)
                transport = new MediatorRequestResponseTransport(settings);
            else
            {
                var inMemoryOptionSet = new InMemoryOptionSet();
                inMemoryOptionSet.Parse(_remaining);

                inMemoryOptionSet.ShowOptions();

                transport = new InMemoryRequestResponseTransport(inMemoryOptionSet, settings);
            }

            var benchmark = new RequestResponseBenchmark(transport, settings);

            benchmark.Run();
        }

        static async Task RunBusOutboxBenchmark(ProgramOptionSet optionSet)
        {
            var busOutboxBenchmarkOptions = new BusOutboxBenchmarkOptions();

            busOutboxBenchmarkOptions.Parse(_remaining);

            IConfigureBusOutboxTransport transport;
            if (optionSet.Transport == ProgramOptionSet.TransportOptions.RabbitMq)
            {
                var rabbitMqOptionSet = new RabbitMqOptionSet();
                rabbitMqOptionSet.Parse(_remaining);

                rabbitMqOptionSet.ShowOptions();

                transport = new RabbitMqConfigureBusOutboxTransport(rabbitMqOptionSet, busOutboxBenchmarkOptions);
            }
            else if (optionSet.Transport == ProgramOptionSet.TransportOptions.AzureServiceBus)
            {
                var serviceBusOptionSet = new ServiceBusOptionSet();

                serviceBusOptionSet.Parse(_remaining);

                serviceBusOptionSet.ShowOptions();

                ServicePointManager.Expect100Continue = false;
                ServicePointManager.UseNagleAlgorithm = false;

                transport = new ServiceBusConfigureBusOutboxTransport(serviceBusOptionSet, busOutboxBenchmarkOptions);
            }
            else
            {
                var inMemoryOptionSet = new InMemoryOptionSet();
                inMemoryOptionSet.Parse(_remaining);

                inMemoryOptionSet.ShowOptions();

                transport = new InMemoryConfigureBusOutboxTransport(inMemoryOptionSet, busOutboxBenchmarkOptions);
            }

            var benchmark = new BusOutboxBenchmark(transport, busOutboxBenchmarkOptions);

            await benchmark.Run();
        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: mtbench [OPTIONS]+");
            Console.WriteLine("Executes the benchmark using the specified transport with the specified options.");
            Console.WriteLine("If no benchmark is specified, all benchmarks are executed.");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);

            Console.WriteLine();
            Console.WriteLine("RabbitMQ Options:");
            new RabbitMqOptionSet().WriteOptionDescriptions(Console.Out);

            Console.WriteLine();
            Console.WriteLine("Azure Service Bus Options:");
            new ServiceBusOptionSet().WriteOptionDescriptions(Console.Out);

            Console.WriteLine();
            Console.WriteLine("Amazon SQS Options:");
            new AmazonSqsOptionSet().WriteOptionDescriptions(Console.Out);

            Console.WriteLine();
            Console.WriteLine("Benchmark Options:");
            new MessageLatencyOptionSet().WriteOptionDescriptions(Console.Out);
        }
    }
}
