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
        public async Task Should_use_the_new_syntax_to_be_awesome()
        {
            IRequestClient<Request> client = Bus.CreateRequestClient<Request>();

            Response<ResponseA, ResponseB> response = await client.GetResponse<ResponseA, ResponseB>(new Request());

            if (response.Is(out Response<ResponseA> responseA))
            {
                Assert.Fail("Should have been responseB");
            }
            else if (response.Is(out Response<ResponseB> responseB))
            {
                Assert.Pass("All good");
            }
            else
            {
                Assert.Fail("No match");
            }
        }

        [Test]
        public async Task Should_use_the_new_syntax_to_be_awesome_er()
        {
            IRequestClient<Request> client = Bus.CreateRequestClient<Request>();

            Response response = await client.GetResponse<ResponseA, ResponseB>(new Request());

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

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Handler<Request>(async context =>
            {
                await context.RespondAsync(new ResponseB());
            });
        }


        class Request
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
