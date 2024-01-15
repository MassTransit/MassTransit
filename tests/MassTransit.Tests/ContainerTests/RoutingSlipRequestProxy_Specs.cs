namespace MassTransit.Tests.ContainerTests
{
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using RoutingSlipProxySubjects;
    using TestFramework.Courier;


    [TestFixture]
    public class Using_the_routing_slip_request_proxy
    {
        [Test]
        public async Task Should_support_retry_options()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.SetKebabCaseEndpointNameFormatter();
                    x.AddSingleton<IEndpointAddressProvider, InMemoryEndpointAddressProvider>();

                    x.AddConsumer<RoutingRequestConsumer>();
                    x.AddConsumer<RoutingResponseConsumer>();

                    x.AddActivity<TestActivity, TestArguments, TestLog>();
                    x.AddActivity<SecondTestActivity, TestArguments, TestLog>();
                    x.AddActivity<FaultyActivity, FaultyArguments, FaultyLog>();
                })
                .BuildServiceProvider(true);

            var harness = await provider.StartTestHarness();

            IRequestClient<RoutingRequest> client = harness.GetRequestClient<RoutingRequest>();

            Assert.That(async () => await client.GetResponse<RoutingResponse>(new RoutingRequest()), Throws.TypeOf<RequestFaultException>());

            Assert.That(await harness.Sent.Any<Fault<RoutingRequest>>());

            var fault = await harness.Sent.SelectAsync<Fault<RoutingRequest>>().First();

            Assert.That(fault.Context.Headers.Get<int>(MessageHeaders.FaultRetryCount), Is.EqualTo(1));
        }
    }


    namespace RoutingSlipProxySubjects
    {
        using System;
        using MassTransit.Courier;
        using MassTransit.Courier.Contracts;


        public interface IEndpointAddressProvider
        {
            Uri GetExecuteEndpoint<T, TArguments>()
                where T : class, IExecuteActivity<TArguments>
                where TArguments : class;

            Uri GetResponseConsumerEndpoint<TConsumer, TRequest, TResponse>()
                where TConsumer : RoutingSlipResponseProxy<TRequest, TResponse>
                where TResponse : class
                where TRequest : class;
        }


        public class InMemoryEndpointAddressProvider :
            IEndpointAddressProvider
        {
            readonly IEndpointNameFormatter _formatter;

            public InMemoryEndpointAddressProvider(IEndpointNameFormatter formatter)
            {
                _formatter = formatter;
            }

            public Uri GetExecuteEndpoint<T, TArguments>()
                where T : class, IExecuteActivity<TArguments>
                where TArguments : class
            {
                return new Uri($"exchange:{_formatter.ExecuteActivity<T, TArguments>()}");
            }

            public Uri GetResponseConsumerEndpoint<TConsumer, TRequest, TResponse>()
                where TConsumer : RoutingSlipResponseProxy<TRequest, TResponse>
                where TRequest : class
                where TResponse : class
            {
                return new Uri($"exchange:{_formatter.Consumer<TConsumer>()}");
            }
        }


        record RoutingRequest
        {
        }


        record RoutingResponse
        {
        }


        class RoutingRequestConsumer :
            RoutingSlipRequestProxy<RoutingRequest>
        {
            readonly IEndpointAddressProvider _endpointAddressProvider;

            public RoutingRequestConsumer(IEndpointAddressProvider endpointAddressProvider)
            {
                _endpointAddressProvider = endpointAddressProvider;
            }

            protected override async Task BuildRoutingSlip(RoutingSlipBuilder builder, ConsumeContext<RoutingRequest> request)
            {
                builder.AddActivity(nameof(TestActivity), _endpointAddressProvider.GetExecuteEndpoint<TestActivity, TestArguments>(), new
                {
                    Value = "Hello",
                    NullValue = (string)null
                });

                builder.AddActivity(nameof(SecondTestActivity), _endpointAddressProvider.GetExecuteEndpoint<SecondTestActivity, TestArguments>(),
                    new
                    {
                        Value = "Hello Again",
                        NullValue = (string)null
                    });

                builder.AddActivity(nameof(FaultyActivity), _endpointAddressProvider.GetExecuteEndpoint<FaultyActivity, FaultyArguments>(),
                    new
                    {
                        Value = "Hello Again",
                        NullValue = (string)null
                    });
            }

            protected override Uri GetResponseEndpointAddress(ConsumeContext<RoutingRequest> context)
            {
                return _endpointAddressProvider.GetResponseConsumerEndpoint<RoutingResponseConsumer, RoutingRequest, RoutingResponse>();
            }
        }


        class RoutingResponseConsumer :
            RoutingSlipResponseProxy<RoutingRequest, RoutingResponse>
        {
            public RoutingResponseConsumer()
            {
                RetryPolicy = Retry.Immediate(1);
            }

            protected override IRetryPolicy RetryPolicy { get; }

            protected override Task<RoutingResponse> CreateResponseMessage(ConsumeContext<RoutingSlipCompleted> context, RoutingRequest request)
            {
                return Task.FromResult(new RoutingResponse());
            }
        }
    }
}
