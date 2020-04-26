namespace MassTransit.AspNetCoreIntegration.Tests
{
    using System.Threading.Tasks;
    using GreenPipes;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;


    public class SendScope_Specs :
        AspNetCoreInMemoryTestFixture
    {
        const string Endpoint = "/ping";
        readonly TaskCompletionSource<MyId> _httpContextIdTask;
        readonly TaskCompletionSource<MyId> _sendContextIdTask;

        public SendScope_Specs()
        {
            _httpContextIdTask = GetTask<MyId>();
            _sendContextIdTask = GetTask<MyId>();
        }

        [Test]
        public async Task Should_use_http_context_scope_if_exists()
        {
            var client = GetHttpClient();
            await client.GetAsync(Endpoint);

            var httpContext = await _httpContextIdTask.Task;
            var sendContext = await _sendContextIdTask.Task;

            Assert.AreEqual(httpContext, sendContext);
        }

        protected override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
        {
            base.ConfigureServices(context, services);
            services.AddHttpContextAccessor();
            services.AddScoped<MyId>();
        }

        protected override void Configure(WebHostBuilderContext context, IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(builder => builder.Map(Endpoint, async httpContext =>
            {
                var sendEndpointProvider = httpContext.RequestServices.GetService<ISendEndpointProvider>();
                var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(InputQueueAddress);

                _httpContextIdTask.TrySetResult(httpContext.RequestServices.GetService<MyId>());

                await sendEndpoint.Send<IPing>(new { }, httpContext.RequestAborted);

                await httpContext.Response.WriteAsync("pong");
            }));
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureSend(cfg => cfg.UseFilter(new TestScopeFilter(_sendContextIdTask)));
        }

        class TestScopeFilter : IFilter<SendContext>
        {
            readonly TaskCompletionSource<MyId> _taskCompletionSource;

            public TestScopeFilter(TaskCompletionSource<MyId> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Send(SendContext context, IPipe<SendContext> next)
            {
                if (context.TryGetPayload<IServiceScope>(out var scope))
                    _taskCompletionSource.TrySetResult(scope.ServiceProvider.GetService<MyId>());
                await next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
            }
        }
    }
}
