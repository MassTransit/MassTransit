namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Turnout.Contracts;


    [TestFixture]
    public class Creating_a_turnout_job :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_allow_scheduling_a_job()
        {
            var endpoint = await Bus.GetSendEndpoint(_commandEndpointAddress);

            await endpoint.Send(new ProcessFile
            {
                Filename = "log.txt",
                Size = 1
            });

            ConsumeContext<JobCompleted> context = await _completed;

            await endpoint.Send(new ProcessFile
            {
                Filename = "log.txt",
                Size = 2
            });

            context = await _completed2;
        }


        public class ProcessFile
        {
            public string Filename { get; set; }
            public int Size { get; set; }
        }


        Task<ConsumeContext<JobCompleted>> _completed;
        Uri _commandEndpointAddress;
        Task<ConsumeContext<JobCompleted>> _completed2;

        protected override void ConfigureRabbitMqBusHost(IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host)
        {
            configurator.UseDelayedExchangeMessageScheduler();

            base.ConfigureRabbitMqBusHost(configurator, host);

            configurator.TurnoutEndpoint<ProcessFile>("process_queue", endpoint =>
            {
                endpoint.SuperviseInterval = TimeSpan.FromSeconds(1);
                endpoint.SetJobFactory(async context => await Task.Delay(TimeSpan.FromSeconds(context.Command.Size)).ConfigureAwait(false));

                _commandEndpointAddress = endpoint.InputAddress;
            });
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _completed = Handled<JobCompleted>(configurator, context => context.Message.GetArguments<ProcessFile>().Size == 1);
            _completed2 = Handled<JobCompleted>(configurator, context => context.Message.GetArguments<ProcessFile>().Size == 2);
        }
    }

    [TestFixture]
    public class Stopping_the_bus_before_the_job_is_done :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_send_the_job_canceled()
        {
            var endpoint = await Bus.GetSendEndpoint(_commandEndpointAddress);

            await endpoint.Send(new ProcessFile
            {
                Filename = "log.txt",
                Size = 10
            });

            var context = await _started;

            Console.WriteLine("Started on address: {0}", context.Message.ManagementAddress);
        }


        public class ProcessFile
        {
            public string Filename { get; set; }
            public int Size { get; set; }
        }


        Task<ConsumeContext<JobStarted>> _started;
        Uri _commandEndpointAddress;

        protected override void ConfigureRabbitMqBusHost(IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host)
        {
            configurator.UseDelayedExchangeMessageScheduler();

            base.ConfigureRabbitMqBusHost(configurator, host);

            configurator.TurnoutEndpoint<ProcessFile>("process_queue", endpoint =>
            {
                endpoint.SuperviseInterval = TimeSpan.FromSeconds(1);
                endpoint.SetJobFactory(async context =>
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(context.Command.Size), context.CancellationToken).ConfigureAwait(false);
                    }
                    catch (TaskCanceledException)
                    {
                        Console.WriteLine("Task was canceled!");
                        throw;
                    }
                });

                _commandEndpointAddress = endpoint.InputAddress;
            });
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _started = Handled<JobStarted>(configurator);
        }
    }

    [TestFixture]
    public class Cancelling_a_job_using_the_management_address :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_send_the_job_canceled()
        {
            var endpoint = await Bus.GetSendEndpoint(_commandEndpointAddress);

            await endpoint.Send(new ProcessFile
            {
                Filename = "log.txt",
                Size = 10
            });

            var context = await _started;

            Console.WriteLine("Started on address: {0}", context.Message.ManagementAddress);

            var managementEndpoint = await Bus.GetSendEndpoint(context.Message.ManagementAddress);

            await managementEndpoint.Send<CancelJob>(new
            {
                JobId = context.Message.JobId,
                Timestamp = DateTime.UtcNow,
                Reason = "Impatient!"
            });

            await _canceled;

            Console.WriteLine("Canceled, yea!");
        }


        public class ProcessFile
        {
            public string Filename { get; set; }
            public int Size { get; set; }
        }


        Task<ConsumeContext<JobStarted>> _started;
        Uri _commandEndpointAddress;
        Task<ConsumeContext<JobCanceled<ProcessFile>>> _canceled;

        protected override void ConfigureRabbitMqBusHost(IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host)
        {
            configurator.UseDelayedExchangeMessageScheduler();

            configurator.TurnoutEndpoint<ProcessFile>("process_queue", endpoint =>
            {
                endpoint.SuperviseInterval = TimeSpan.FromSeconds(1);
                endpoint.SetJobFactory(async context =>
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(context.Command.Size), context.CancellationToken).ConfigureAwait(false);
                    }
                    catch (TaskCanceledException)
                    {
                        Console.WriteLine("Task was canceled!");
                        throw;
                    }
                });

                _commandEndpointAddress = endpoint.InputAddress;
            });
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _started = Handled<JobStarted>(configurator);
            _canceled = Handled<JobCanceled<ProcessFile>>(configurator);
        }
    }
}
