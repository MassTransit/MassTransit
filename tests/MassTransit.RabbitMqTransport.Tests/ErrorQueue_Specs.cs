namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;
    using Metadata;
    using NUnit.Framework;
    using RabbitMQ.Client;
    using Shouldly;
    using TestFramework.Messages;


    [TestFixture]
    public class A_serialization_exception :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_have_the_correlation_id()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            context.CorrelationId.ShouldBe(_correlationId);
        }

        [Test]
        public async Task Should_have_the_exception()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            context.ReceiveContext.TransportHeaders.Get("MT-Fault-Message", (string)null).ShouldBe("This is fine, forcing death");
        }

        [Test]
        public async Task Should_have_the_host_machine_name()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            context.ReceiveContext.TransportHeaders.Get("MT-Host-MachineName", (string)null).ShouldBe(HostMetadataCache.Host.MachineName);
        }

        [Test]
        public async Task Should_have_the_original_destination_address()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            context.DestinationAddress.ShouldBe(InputQueueAddress);
        }

        [Test]
        public async Task Should_have_the_original_fault_address()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            context.FaultAddress.ShouldBe(BusAddress);
        }

        [Test]
        public async Task Should_have_the_original_response_address()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            context.ResponseAddress.ShouldBe(BusAddress);
        }

        [Test]
        public async Task Should_have_the_original_source_address()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            context.SourceAddress.ShouldBe(BusAddress);
        }

        [Test]
        public async Task Should_have_the_reason()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            context.ReceiveContext.TransportHeaders.Get("MT-Reason", (string)null).ShouldBe("fault");
        }

        [Test]
        public async Task Should_move_the_message_to_the_error_queue()
        {
            await _errorHandler;
        }

        Task<ConsumeContext<PingMessage>> _errorHandler;
        readonly Guid? _correlationId = NewId.NextGuid();

        [OneTimeSetUp]
        public async Task Setup()
        {
            await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext<PingMessage>>(context =>
            {
                context.CorrelationId = _correlationId;
                context.ResponseAddress = Bus.Address;
                context.FaultAddress = Bus.Address;
            }));
        }

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("input_queue_error", x =>
            {
                x.PurgeOnStartup = true;

                _errorHandler = Handled<PingMessage>(x);
            });
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            Handler<PingMessage>(configurator, context => throw new SerializationException("This is fine, forcing death"));
        }
    }


    [TestFixture]
    public class A_serialization_exception_from_a_bad_message :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_have_the_host_machine_name()
        {
            var header = Encoding.UTF8.GetString((byte[])_basicGetResult.BasicProperties.Headers["MT-Host-MachineName"]);
            header.ShouldBe(HostMetadataCache.Host.MachineName);
        }

        [Test]
        public void Should_have_the_invalid_body()
        {
            _body.ShouldBe("[]");
        }

        [Test]
        public async Task Should_have_the_reason()
        {
            var header = Encoding.UTF8.GetString((byte[])_basicGetResult.BasicProperties.Headers["MT-Reason"]);

            header.ShouldBe("fault");
        }

        string _body;
        BasicGetResult _basicGetResult;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var connectionFactory = GetHostSettings().GetConnectionFactory();
            using (var connection = connectionFactory.CreateConnection())
            using (var model = connection.CreateModel())
            {
                var bytes = Encoding.UTF8.GetBytes("[]");

                model.BasicPublish("input_queue", "", model.CreateBasicProperties(), bytes);

                await Task.Delay(3000).ConfigureAwait(false);

                _basicGetResult = model.BasicGet("input_queue_error", true);

                _body = Encoding.UTF8.GetString(_basicGetResult.Body.ToArray());

                model.Close(200, "Cleanup complete");
                connection.Close(200, "Cleanup complete");
            }
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            Handled<PingMessage>(configurator);
        }
    }


    [TestFixture]
    public class An_empty_message_body :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_have_the_host_machine_name()
        {
            var header = Encoding.UTF8.GetString((byte[])_basicGetResult.BasicProperties.Headers["MT-Host-MachineName"]);
            header.ShouldBe(HostMetadataCache.Host.MachineName);
        }

        [Test]
        public void Should_have_the_invalid_body()
        {
            _body.ShouldBe("");
        }

        [Test]
        public async Task Should_have_the_reason()
        {
            var header = Encoding.UTF8.GetString((byte[])_basicGetResult.BasicProperties.Headers["MT-Reason"]);

            header.ShouldBe("fault");
        }

        string _body;
        BasicGetResult _basicGetResult;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var connectionFactory = GetHostSettings().GetConnectionFactory();
            using (var connection = connectionFactory.CreateConnection())
            using (var model = connection.CreateModel())
            {
                model.BasicPublish("input_queue", "", model.CreateBasicProperties(), null);

                await Task.Delay(5000).ConfigureAwait(false);

                _basicGetResult = model.BasicGet("input_queue_error", true);

                _body = Encoding.UTF8.GetString(_basicGetResult.Body.ToArray());

                model.Close(200, "Cleanup complete");
                connection.Close(200, "Cleanup complete");
            }
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.PrefetchCount = 1;
            configurator.UseConcurrencyLimit(1);

            Handled<PingMessage>(configurator);
        }
    }


    [TestFixture]
    public class A_request_client_exception :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_have_the_correlation_id()
        {
            Assert.That(async () => await _responseTask, Throws.TypeOf<RequestFaultException>());
        }

        [Test]
        public async Task Should_have_the_exception()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            Assert.That(context.ReceiveContext.TransportHeaders.Get("MT-Fault-Message", (string)null), Is.EqualTo("Request is so bad, I'm dying here!"));
        }

        [Test]
        public async Task Should_have_the_original_destination_address()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            Assert.That(context.DestinationAddress, Is.EqualTo(InputQueueAddress));
        }

        [Test]
        public async Task Should_have_the_original_response_address()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            Assert.That(context.ResponseAddress, Is.EqualTo(BusAddress));
        }

        [Test]
        public async Task Should_have_the_original_source_address()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            Assert.That(context.SourceAddress, Is.EqualTo(BusAddress));
        }

        [Test]
        public async Task Should_have_the_reason()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            Assert.That(context.ReceiveContext.TransportHeaders.Get("MT-Reason", (string)null), Is.EqualTo("fault"));
        }

        [Test]
        public async Task Should_move_the_message_to_the_error_queue()
        {
            await _errorHandler;
        }

        [Test]
        public async Task Write_out_message_body()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            using (var body = context.ReceiveContext.GetBodyStream())
            using (var output = new MemoryStream())
            {
                await body.CopyToAsync(output);
                await output.FlushAsync();

                var text = Encoding.UTF8.GetString(output.ToArray());
                Console.WriteLine(text);
            }
        }

        Task<ConsumeContext<PingMessage>> _errorHandler;
        Task<Response<PongMessage>> _responseTask;

        [OneTimeSetUp]
        public async Task Setup()
        {
            IRequestClient<PingMessage> client = Bus.CreateRequestClient<PingMessage>(InputQueueAddress, TestTimeout);

            _responseTask = client.GetResponse<PongMessage>(new PingMessage());
        }

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("input_queue_error", x =>
            {
                x.PurgeOnStartup = true;

                _errorHandler = Handled<PingMessage>(x);
            });
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            Handler<PingMessage>(configurator, context => throw new Exception("Request is so bad, I'm dying here!"));
        }
    }


    [TestFixture]
    public class An_aggregate_exception :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_have_the_actual_exception()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            Assert.That(context.ReceiveContext.TransportHeaders.Get("MT-Fault-Message", (string)null), Is.EqualTo("Request is so bad, I'm dying here!"));
        }

        [Test]
        public async Task Should_move_the_message_to_the_error_queue()
        {
            await _errorHandler;
        }

        Task<ConsumeContext<PingMessage>> _errorHandler;
        Task<Response<PongMessage>> _responseTask;

        [OneTimeSetUp]
        public async Task Setup()
        {
            IRequestClient<PingMessage> client = Bus.CreateRequestClient<PingMessage>(InputQueueAddress, TestTimeout);

            _responseTask = client.GetResponse<PongMessage>(new PingMessage());
        }

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("input_queue_error", x =>
            {
                x.PurgeOnStartup = true;

                _errorHandler = Handled<PingMessage>(x);
            });
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            Handler<PingMessage>(configurator, async context =>
            {
                throw new AggregateException("Request is so bad, I'm dying here!");
            });
        }
    }
}
