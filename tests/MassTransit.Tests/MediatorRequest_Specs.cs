namespace MassTransit.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using Mediator;
    using Microsoft.Extensions.DependencyInjection;
    using Middleware;
    using NUnit.Framework;


    [TestFixture]
    public class MediatorRequest_Specs
    {
        [Test]
        public async Task Should_support_the_request_style()
        {
            await using var provider = new ServiceCollection()
                .AddMediator(cfg =>
                {
                    cfg.AddConsumer<UserRequestConsumer>();
                })
                .BuildServiceProvider(true);

            var mediator = provider.GetRequiredService<IMediator>();

            var user = await mediator.SendRequest(new UserFromUsername { Username = "phatboyg" });

            Assert.That(user.Username, Is.EqualTo("phatboyg"));

            user = await mediator.SendRequest(new UserFromEmail { Email = "phatboyg@gmail.com" });

            Assert.That(user.Username, Is.EqualTo("phatboyg"));
        }

        [Test]
        public async Task Should_throw_exceptions_when_bad_things_happen()
        {
            await using var provider = new ServiceCollection()
                .AddMediator(cfg =>
                {
                    cfg.AddConsumer<UserRequestConsumer>();
                })
                .BuildServiceProvider(true);

            var mediator = provider.GetRequiredService<IMediator>();

            Assert.That(async () => await mediator.SendRequest(new UserFromUsername { Username = "missing" }), Throws.TypeOf<IntentionalTestException>());
        }
    }


    public class UserFromEmail :
        Request<User>
    {
        public string Email { get; set; }
    }


    public class UserFromUsername :
        Request<User>
    {
        public string Username { get; set; }
    }


    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }


    public class UserRequestConsumer :
        IConsumer<UserFromEmail>,
        IConsumer<UserFromUsername>
    {
        public async Task Consume(ConsumeContext<UserFromEmail> context)
        {
            await context.RespondAsync(new User
            {
                Email = context.Message.Email,
                Id = 123,
                Username = context.Message.Email.Split('@').FirstOrDefault() ?? "unknown"
            });
        }

        public async Task Consume(ConsumeContext<UserFromUsername> context)
        {
            if (context.Message.Username == "missing")
                throw new IntentionalTestException("User not found: missing");

            await context.RespondAsync(new User
            {
                Email = context.Message.Username + "@compuserve.net",
                Id = 123,
                Username = context.Message.Username
            });
        }
    }
}
