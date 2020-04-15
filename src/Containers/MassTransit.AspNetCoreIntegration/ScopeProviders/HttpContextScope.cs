namespace MassTransit.AspNetCoreIntegration.ScopeProviders
{
    using System;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;


    class HttpContextScope : IServiceScope
    {
        readonly HttpContext _httpContext;

        public HttpContextScope(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public void Dispose()
        {
        }

        public IServiceProvider ServiceProvider => _httpContext.RequestServices;
    }
}
