namespace MassTransit.AspNetCoreIntegration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework;


    public class AspNetCoreInMemoryTestFixture :
        InMemoryTestFixture
    {
        readonly TestServer _server;
        readonly List<HttpClient> _clients;

        public AspNetCoreInMemoryTestFixture()
        {
            _server = new TestServer(WebHostBuilder(Array.Empty<string>()));
            _clients = new List<HttpClient>();
        }

        [SetUp]
        public void SetupTestServer()
        {
        }

        [TearDown]
        public void TearDownTestServer()
        {
            _clients.ForEach(x => x.Dispose());
            _server?.Dispose();
        }

        protected IServiceProvider GetServiceProvider() => _server.Services;

        protected HttpClient GetHttpClient()
        {
            var httpClient = _server.CreateClient();
            _clients.Add(httpClient);
            return httpClient;
        }

        protected virtual void Configure(WebHostBuilderContext context, IApplicationBuilder app)
        {
        }

        protected virtual void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
        {
            services.AddMassTransit(configurator =>
            {
                configurator.AddBus(_ => BusControl);
            });
        }

        IWebHostBuilder WebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(ConfigureServices)
                .Configure(Configure);
    }
}
