namespace MassTransit.Tests
{
    using MassTransit.Mediator;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    [TestFixture]
    public class MediatorNullableResponse_Specs
    {
        [Test]
        public async Task Should_return_the_response_by_default()
        {
            // Arrange
            await using var provider = new ServiceCollection()
                .AddMediator(cfg =>
                {
                    cfg.AddConsumer<SampleRequestHandler>();
                })
                .BuildServiceProvider(validateScopes: true);

            var mediator = provider.GetRequiredService<IMediator>();

            // Act
            var response = await mediator.SendRequest(new SampleRequest());

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response, Is.TypeOf<SampleResponse>());
        }

        [Test]
        public async Task Should_return_null_when_response_is_null()
        {
            // Arrange
            await using var provider = new ServiceCollection()
                .AddMediator(cfg =>
                {
                    cfg.AddConsumer<SampleRequestHandler>();
                })
                .BuildServiceProvider(validateScopes: true);

            var mediator = provider.GetRequiredService<IMediator>();

            // Act
            var response = await mediator.SendRequest(new SampleRequest(new RequestConfiguration { ReturnNull = true }));

            // Assert
            Assert.That(response, Is.Null);
        }

        [Test]
        public async Task Should_throw_request_exception_when_request_fails()
        {
            // Arrange
            await using var provider = new ServiceCollection()
                .AddMediator(cfg =>
                {
                    cfg.AddConsumer<SampleRequestHandler>();
                })
                .BuildServiceProvider(validateScopes: true);

            var mediator = provider.GetRequiredService<IMediator>();

            // Act
            Task action() => mediator.SendRequest(new SampleRequest(new RequestConfiguration { ThrowRequestException = true }));

            // Assert
            Assert.That(action, Throws.TypeOf<RequestException>());
        }

        [Test]
        public async Task Should_throw_the_inner_exception_when_there_is_one()
        {
            // Arrange
            await using var provider = new ServiceCollection()
                .AddMediator(cfg =>
                {
                    cfg.AddConsumer<SampleRequestHandler>();
                })
                .BuildServiceProvider(validateScopes: true);

            var mediator = provider.GetRequiredService<IMediator>();

            // Act
            Task action() => mediator.SendRequest(new SampleRequest(new RequestConfiguration { ThrowApplicationException = true }));

            // Assert
            Assert.That(action, Throws.TypeOf<ApplicationException>());
        }

        #region Fixtures
        class RequestConfiguration
        {
            public bool ReturnNull { get; set; }
            public bool ThrowRequestException { get; set; }
            public bool ThrowApplicationException { get; set; }
        }

        class SampleRequest : Request<SampleResponse>
        {
            public SampleRequest() : this(new RequestConfiguration()) { }
            public SampleRequest(RequestConfiguration configuration) => Configuration = configuration;

            public RequestConfiguration Configuration { get; }
        }

        class SampleResponse { }

        class SampleRequestHandler : MediatorRequestHandler<SampleRequest, SampleResponse>
        {
            protected override Task<SampleResponse> Handle(SampleRequest request, CancellationToken cancellationToken)
            {
                if (request.Configuration.ThrowRequestException)
                {
                    throw new RequestException();
                }

                if (request.Configuration.ThrowApplicationException)
                {
                    throw new ApplicationException();
                }

                if (request.Configuration.ReturnNull)
                {
                    return Task.FromResult<SampleResponse>(null);
                }

                return Task.FromResult(new SampleResponse());
            }
        }
        #endregion
    }
}
