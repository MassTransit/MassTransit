namespace MassTransit.Containers.Tests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Testing;


    [TestFixture]
    public class When_a_base_type_is_excluded
    {
        [Test]
        public async Task The_filter_should_only_be_called_once()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<TenantConsumer>();

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


        class TenantConsumer :
            IConsumer<TenantRequest>
        {
            public Task Consume(ConsumeContext<TenantRequest> context)
            {
                if (!context.TryGetHeader("X-FilterCount", out int? value))
                    throw new InvalidOperationException("Header not found.");

                if (value != 1)
                    throw new InvalidOperationException($"The header value was not one: {value}");

                return context.RespondAsync(new TenantResponse());
            }
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
                var headerValue = context.TryGetHeader("X-FilterCount", out int? value) ? value + 1 : 1;

                context.Headers.Set("X-FilterCount", headerValue);

                _logger.LogInformation($"Set filter count to {headerValue}");

                return next.Send(context);
            }
        }


        [Serializable]
        public class TenantRequest :
            IRequest
        {
        }


        [ExcludeFromImplementedTypes]
        public interface IRequest
        {
        }


        [Serializable]
        public class TenantResponse
        {
        }
    }
}
