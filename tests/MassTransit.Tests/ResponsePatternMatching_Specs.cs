namespace MassTransit.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class ResponsePatternMatching_Specs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_fail_with_unsupported_response_type()
        {
            Assert.That(async () => await _client.GetResponse<ResponseA>(new Request()), Throws.TypeOf<RequestFaultException>());
        }

        [Test]
        public async Task Should_pass_with_a_single_response_type()
        {
            Response<ResponseB> response = await _client.GetResponse<ResponseB>(new Request());
        }

        [Test]
        public async Task Should_use_the_new_syntax_to_be_awesome()
        {
            Response<ResponseA, ResponseB> response = await _client.GetResponse<ResponseA, ResponseB>(new Request());

            if (response.Is(out Response<ResponseA> responseA))
                Assert.Fail("Should have been responseB");
            else if (response.Is(out Response<ResponseB> responseB))
                Assert.Pass("All good");
            else
                Assert.Fail("No match");
        }

        [Test]
        public async Task Should_use_the_new_syntax_to_be_awesome_er()
        {
            Response response = await _client.GetResponse<ResponseA, ResponseB>(new Request());

            switch (response)
            {
                case (_, ResponseA a) responseA:
                    Assert.Fail("Should have been responseB");
                    break;
                case (_, ResponseB b) responseB:
                    Assert.Pass("All good");
                    break;
                default:
                    Assert.Fail("No pattern matched");
                    break;
            }

            Assert.That(response switch
            {
                (_, ResponseA a) => false,
                (_, ResponseB b) => true,
                _ => throw new InvalidOperationException()
            }, Is.True);
        }

        IRequestClient<Request> _client;

        [OneTimeSetUp]
        public void SetupClient()
        {
            _client = Bus.CreateRequestClient<Request>();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Handler<Request>(context =>
            {
                if (context.IsResponseAccepted<ResponseB>(false))
                    return context.RespondAsync(new ResponseB());

                throw new InvalidOperationException("The response type was not supported by the request");
            });
        }


        public class Request
        {
        }


        class ResponseA
        {
        }


        class ResponseB
        {
        }


        class ResponseC
        {
        }
    }


    [TestFixture]
    public class MediatorResponsePatternMatching_Specs :
        MediatorTestFixture
    {
        [Test]
        public async Task Should_fail_with_unsupported_response_type()
        {
            Assert.That(async () => await _client.GetResponse<ResponseA>(new Request()), Throws.TypeOf<RequestException>());
        }

        [Test]
        public async Task Should_pass_with_a_single_response_type()
        {
            Response<ResponseB> response = await _client.GetResponse<ResponseB>(new Request());
        }

        [Test]
        public async Task Should_use_the_new_syntax_to_be_awesome()
        {
            Response<ResponseA, ResponseB> response = await _client.GetResponse<ResponseA, ResponseB>(new Request());

            if (response.Is(out Response<ResponseA> responseA))
                Assert.Fail("Should have been responseB");
            else if (response.Is(out Response<ResponseB> responseB))
                Assert.Pass("All good");
            else
                Assert.Fail("No match");
        }

        [Test]
        public async Task Should_use_the_new_syntax_to_be_awesome_er()
        {
            Response response = await _client.GetResponse<ResponseA, ResponseB>(new Request());

            switch (response)
            {
                case (_, ResponseA a) responseA:
                    Assert.Fail("Should have been responseB");
                    break;
                case (_, ResponseB b) responseB:
                    Assert.Pass("All good");
                    break;
                default:
                    Assert.Fail("No pattern matched");
                    break;
            }

            Assert.That(response switch
            {
                (_, ResponseA a) => false,
                (_, ResponseB b) => true,
                _ => throw new InvalidOperationException()
            }, Is.True);
        }

        IRequestClient<Request> _client;

        [OneTimeSetUp]
        public void SetupClient()
        {
            _client = CreateRequestClient<Request>();
        }

        protected override void ConfigureMediator(IMediatorConfigurator configurator)
        {
            configurator.Handler<Request>(context =>
            {
                if (context.IsResponseAccepted<ResponseB>(false))
                    return context.RespondAsync(new ResponseB());

                throw new InvalidOperationException("The response type was not supported by the request");
            });
        }


        public class Request
        {
        }


        class ResponseA
        {
        }


        class ResponseB
        {
        }


        class ResponseC
        {
        }
    }
}
