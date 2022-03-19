namespace MassTransit.Containers.Tests
{
    using System;
    using System.Threading.Tasks;
    using Courier;
    using Courier.Contracts;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using TestFramework;
    using Testing;


    [TestFixture]
    public class When_specifying_a_scoped_filter
    {
        [Test]
        public async Task It_should_be_resolved_prior_to_resolving_the_consumer()
        {
            await using var provider = new ServiceCollection()
                .AddScoped<TenantContext>()
                .AddScoped<FakeTenantDbContext>()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<TenantConsumer>();

                    x.AddConfigureEndpointsCallback((provider, name, cfg) =>
                    {
                        cfg.UseConsumeFilter(typeof(TenantConsumeContextFilter<>), provider);
                    });

                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.UsePublishFilter(typeof(PublishTenantHeaderFilter<>), context);

                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            IRequestClient<TenantRequest> client = harness.GetRequestClient<TenantRequest>();

            await client.GetResponse<TenantResponse>(new TenantRequest());
        }

        [Test]
        public async Task It_should_be_resolved_prior_to_resolving_the_consumer_with_send()
        {
            await using var provider = new ServiceCollection()
                .AddScoped<TenantContext>()
                .AddScoped<FakeTenantDbContext>()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<TenantConsumer>();
                    x.AddRequestClient<TenantRequest>(new Uri($"queue:{DefaultEndpointNameFormatter.Instance.Consumer<TenantConsumer>()}"));

                    x.AddConfigureEndpointsCallback((provider, name, cfg) =>
                    {
                        cfg.UseConsumeFilter(typeof(TenantConsumeContextFilter<>), provider);
                    });

                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.UseSendFilter(typeof(SendTenantHeaderFilter<>), context);

                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            IRequestClient<TenantRequest> client = harness.GetRequestClient<TenantRequest>();

            await client.GetResponse<TenantResponse>(new TenantRequest());
        }

        [Test]
        public async Task It_should_be_resolved_after_the_message_retry_filter()
        {
            await using var provider = new ServiceCollection()
                .AddScoped<TenantContext>()
                .AddScoped<FakeTenantDbContext>()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<TenantConsumer>();
                    x.AddRequestClient<TenantRequest>(new Uri($"queue:{DefaultEndpointNameFormatter.Instance.Consumer<TenantConsumer>()}"));

                    x.AddConfigureEndpointsCallback((provider, name, cfg) =>
                    {
                        cfg.UseConsumeFilter(typeof(TenantConsumeContextFilter<>), provider);
                    });

                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.UseMessageRetry(r => r.Immediate(10));
                        cfg.UseSendFilter(typeof(SendTenantHeaderFilter<>), context);

                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            IRequestClient<TenantRequest> client = harness.GetRequestClient<TenantRequest>();

            await client.GetResponse<TenantResponse>(new TenantRequest() { FailureCount = 2 });
        }

        [Test]
        public async Task It_should_be_resolved_prior_to_resolving_the_activity()
        {
            await using var provider = new ServiceCollection()
                .AddScoped<TenantContext>()
                .AddScoped<FakeTenantDbContext>()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddExecuteActivity<TenantActivity, TenantArguments>();

                    x.AddConfigureEndpointsCallback((provider, name, cfg) =>
                    {
                        cfg.UseExecuteActivityFilter(typeof(TenantExecuteContextFilter<>), provider);
                    });

                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.UseSendFilter(typeof(SendTenantHeaderFilter<>), context);

                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            var builder = new RoutingSlipBuilder(NewId.NextGuid());
            builder.AddActivity("tenant", new Uri($"queue:{DefaultEndpointNameFormatter.Instance.ExecuteActivity<TenantActivity, TenantArguments>()}"));

            await harness.Bus.Execute(builder.Build());

            Assert.IsTrue(await harness.Published.Any<RoutingSlipCompleted>());
        }


        class TenantConsumer :
            IConsumer<TenantRequest>
        {
            readonly FakeTenantDbContext _dbContext;
            readonly TenantContext _tenantContext;

            public TenantConsumer(FakeTenantDbContext dbContext, TenantContext tenantContext, ILogger<TenantConsumer> logger)
            {
                _dbContext = dbContext;
                _tenantContext = tenantContext;

                logger.LogInformation($"{tenantContext.TenantId} - {dbContext.TenantId} Creating TenantConsumer");
            }

            public Task Consume(ConsumeContext<TenantRequest> context)
            {
                if (string.IsNullOrWhiteSpace(_tenantContext.TenantId))
                    throw new InvalidOperationException("The tenantId was not present");

                if (_tenantContext.TenantId != _dbContext.TenantId)
                    throw new InvalidOperationException("The tenantId was not properly initialized prior to consumer resolution");

                if (context.Message.FailureCount > 0)
                {
                    if (context.GetRetryCount() < context.Message.FailureCount)
                        throw new IntentionalTestException("Not yet, not yet.");
                }

                return context.RespondAsync(new TenantResponse());
            }
        }


        public interface TenantArguments
        {
        }


        class TenantActivity :
            IExecuteActivity<TenantArguments>
        {
            readonly FakeTenantDbContext _dbContext;
            readonly TenantContext _tenantContext;

            public TenantActivity(FakeTenantDbContext dbContext, TenantContext tenantContext, ILogger<TenantActivity> logger)
            {
                _dbContext = dbContext;
                _tenantContext = tenantContext;

                logger.LogInformation($"{tenantContext.TenantId} - {dbContext.TenantId} Creating TenantActivity");
            }

            public async Task<ExecutionResult> Execute(ExecuteContext<TenantArguments> context)
            {
                if (string.IsNullOrWhiteSpace(_tenantContext.TenantId))
                    throw new InvalidOperationException("The tenantId was not present");

                if (_tenantContext.TenantId != _dbContext.TenantId)
                    throw new InvalidOperationException("The tenantId was not properly initialized prior to consumer resolution");

                return context.Completed();
            }
        }


        class TenantContext
        {
            public string TenantId { get; set; }
        }


        class FakeTenantDbContext
        {
            public FakeTenantDbContext(TenantContext tenantContext, ILogger<FakeTenantDbContext> logger)
            {
                logger.LogInformation($"{tenantContext.TenantId} Creating FakeTenantDbContext");

                TenantId = tenantContext.TenantId;
            }

            public string TenantId { get; }
        }


        class PublishTenantHeaderFilter<T> :
            IFilter<PublishContext<T>>
            where T : class
        {
            readonly ILogger<PublishTenantHeaderFilter<T>> _logger;

            public PublishTenantHeaderFilter(ILogger<PublishTenantHeaderFilter<T>> logger)
            {
                _logger = logger;
            }

            public void Probe(ProbeContext context)
            {
                context.CreateFilterScope("PublishTenantHeaderFilter");
            }

            public Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
            {
                var tenantId = NewId.NextGuid().ToString();
                _logger.LogInformation($"{tenantId} Setting header for tenant");
                context.Headers.Set("X-TenantId", tenantId);

                return next.Send(context);
            }
        }


        class SendTenantHeaderFilter<T> :
            IFilter<SendContext<T>>
            where T : class
        {
            readonly ILogger<SendTenantHeaderFilter<T>> _logger;

            public SendTenantHeaderFilter(ILogger<SendTenantHeaderFilter<T>> logger)
            {
                _logger = logger;
            }

            public void Probe(ProbeContext context)
            {
                context.CreateFilterScope("SendTenantHeaderFilter");
            }

            public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
            {
                var tenantId = NewId.NextGuid().ToString();
                _logger.LogInformation($"{tenantId} Setting header for tenant");
                context.Headers.Set("X-TenantId", tenantId);

                return next.Send(context);
            }
        }


        class TenantConsumeContextFilter<T> :
            IFilter<ConsumeContext<T>>
            where T : class
        {
            readonly ILogger<TenantConsumeContextFilter<T>> _logger;
            readonly TenantContext _tenantContext;

            public TenantConsumeContextFilter(TenantContext tenantContext, ILogger<TenantConsumeContextFilter<T>> logger)
            {
                _tenantContext = tenantContext;
                _logger = logger;
                _logger.LogInformation("Creating TenantContextFilter");
            }

            public void Probe(ProbeContext context)
            {
                context.CreateScope("TenantConsumeContextFilter");
            }

            public Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
            {
                var tenantId = context.Headers.Get("X-TenantId", string.Empty);
                _tenantContext.TenantId = tenantId;
                _logger.LogInformation($"{tenantId} Reading header for tenant");
                return next.Send(context);
            }
        }


        class TenantExecuteContextFilter<T> :
            IFilter<ExecuteContext<T>>
            where T : class
        {
            readonly ILogger<TenantExecuteContextFilter<T>> _logger;
            readonly TenantContext _tenantContext;

            public TenantExecuteContextFilter(TenantContext tenantContext, ILogger<TenantExecuteContextFilter<T>> logger)
            {
                _tenantContext = tenantContext;
                _logger = logger;
                _logger.LogInformation("Creating TenantContextFilter");
            }

            public void Probe(ProbeContext context)
            {
                context.CreateScope("TenantExecuteContextFilter");
            }

            public Task Send(ExecuteContext<T> context, IPipe<ExecuteContext<T>> next)
            {
                var tenantId = context.Headers.Get("X-TenantId", string.Empty);
                _tenantContext.TenantId = tenantId;
                _logger.LogInformation($"{tenantId} Reading header for tenant");
                return next.Send(context);
            }
        }


        [Serializable]
        class TenantRequest
        {
            public int? FailureCount { get; set; }
        }


        [Serializable]
        class TenantResponse
        {
        }
    }
}
