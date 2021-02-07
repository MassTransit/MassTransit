namespace MassTransit.TestComponents.ForkJoint.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Activities;
    using Conductor;
    using Conductor.Directory;
    using Conductor.Orchestration;
    using Consumers;
    using Contracts;
    using Definition;
    using ExtensionsDependencyInjectionIntegration;
    using Futures;
    using ItineraryPlanners;
    using MassTransit.Futures;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Registration;
    using Services;


    public class Ordering_using_the_request_client :
        FutureTestFixture
    {
        public Ordering_using_the_request_client(IFutureTestFixtureConfigurator testFixtureConfigurator)
            : base(testFixtureConfigurator)
        {
        }

        [Test]
        public async Task Should_complete_the_request_using_futures()
        {
            using var scope = Provider.CreateScope();

            var client = scope.ServiceProvider.GetRequiredService<IRequestClient<SubmitOrder>>();

            var orderId = NewId.NextGuid();
            var fryId = NewId.NextGuid();
            var shakeId = NewId.NextGuid();
            var burgerId = NewId.NextGuid();

            using RequestHandle<SubmitOrder> requestHandle = client.Create(new
            {
                OrderId = orderId,
                Fries = new[]
                {
                    new
                    {
                        FryId = fryId,
                        Size = Size.Large
                    }
                },
                Burgers = new[]
                {
                    new Burger
                    {
                        BurgerId = burgerId,
                        Weight = 1.0m,
                        Cheese = true
                    }
                },
                Shakes = new[]
                {
                    new
                    {
                        ShakeId = shakeId,
                        Size = Size.Large,
                        Flavor = "Chocolate"
                    }
                },
                FryShakes = default(FryShake[])
            });

            requestHandle.TimeToLive = default;

            Response<OrderCompleted> response = await requestHandle.GetResponse<OrderCompleted>();
        }

        protected override void ConfigureServices(IServiceCollection collection)
        {
            collection.AddSingleton<IGrill, Grill>();
            collection.AddScoped<IItineraryPlanner<OrderBurger>, BurgerItineraryPlanner>();
            collection.AddSingleton<IFryer, Fryer>();
            collection.AddSingleton<IShakeMachine, ShakeMachine>();

            collection.AddMassTransitServiceDirectory();
        }

        protected override void ConfigureMassTransit(IServiceCollectionBusConfigurator configurator)
        {
            configurator.AddConsumer<SubmitOrderConsumer, SubmitOrderConsumerDefinition>();

            configurator.AddConsumer<CookFryConsumer, CookFryConsumerDefinition>();
            configurator.AddConsumer<PourShakeConsumer>();
            configurator.AddConsumer<CookOnionRingsConsumer>();

            configurator.AddFuture<BurgerFuture>(typeof(DefaultFutureDefinition<BurgerFuture>));
            configurator.AddFuture<FryFuture>(typeof(DefaultFutureDefinition<FryFuture>));
            configurator.AddFuture<ShakeFuture>(typeof(DefaultFutureDefinition<ShakeFuture>));

            configurator.AddActivitiesFromNamespaceContaining<GrillBurgerActivity>();
        }


        class SubmitOrderConsumer :
            IConsumer<SubmitOrder>
        {
            readonly IOrchestration<Burger, BurgerCompleted> _burgerExecutor;
            readonly IOrchestration<Fry, FryCompleted> _fryExecutor;
            readonly IOrchestration<Shake, ShakeCompleted> _shakeExecutor;

            public SubmitOrderConsumer(IOrchestrationProvider orchestrationProvider)
            {
                _fryExecutor = orchestrationProvider.GetOrchestration<Fry, FryCompleted>();
                _burgerExecutor = orchestrationProvider.GetOrchestration<Burger, BurgerCompleted>();
                _shakeExecutor = orchestrationProvider.GetOrchestration<Shake, ShakeCompleted>();
            }

            public async Task Consume(ConsumeContext<SubmitOrder> context)
            {
                var orchestrationContext = new ConsumeOrchestrationContext<SubmitOrder>(context, context.Message);

                Task<FryCompleted[]> fries = Task.WhenAll(context.Message.Fries?.Select(x => _fryExecutor.Execute(orchestrationContext.Push(x)))
                    ?? Enumerable.Empty<Task<FryCompleted>>());
                Task<BurgerCompleted[]> burgers = Task.WhenAll(context.Message.Burgers?.Select(x => _burgerExecutor.Execute(orchestrationContext.Push(x)))
                    ?? Enumerable.Empty<Task<BurgerCompleted>>());
                Task<ShakeCompleted[]> shakes = Task.WhenAll(context.Message.Shakes?.Select(x => _shakeExecutor.Execute(orchestrationContext.Push(x)))
                    ?? Enumerable.Empty<Task<ShakeCompleted>>());

                await Task.WhenAll(fries, burgers, shakes);

                IEnumerable<OrderLineCompleted> completed = (await fries).Concat((await burgers).Concat((await shakes).Cast<OrderLineCompleted>()));

                await context.RespondAsync<OrderCompleted>(new
                {
                    context.Message.OrderId,
                    Created = context.SentTime ?? DateTime.UtcNow,
                    Completed = DateTime.UtcNow,
                    LinesCompleted = completed.ToDictionary(x => x.OrderLineId)
                });
            }
        }


        class SubmitOrderConsumerDefinition :
            ConsumerDefinition<SubmitOrderConsumer>,
            IConfigureServiceDirectory
        {
            public void Configure(IServiceDirectoryConfigurator configurator)
            {
                configurator.AddService<SubmitOrder, OrderCompleted>(x => x.Consumer<SubmitOrderConsumer>());

                configurator.AddMessageInitializer<Fry, OrderFry>(x => new
                {
                    x.Select<SubmitOrder>().OrderId,
                    OrderLineId = x.Data.FryId,
                    x.Data.Size
                });

                configurator.AddMessageInitializer<Burger, OrderBurger>(x => new
                {
                    x.Select<SubmitOrder>().OrderId,
                    OrderLineId = x.Data.BurgerId,
                    Burger = x.Data
                });

                configurator.AddMessageInitializer<Shake, OrderShake>(x => new
                {
                    x.Select<SubmitOrder>().OrderId,
                    OrderLineId = x.Data.ShakeId,
                    x.Data.Size,
                    x.Data.Flavor
                });

                configurator.AddMessageInitializer<FryReady, FryCompleted>(x => new
                {
                    Created = DateTime.UtcNow,
                    Completed = DateTime.UtcNow,
                    x.Data.OrderId,
                    x.Data.OrderLineId,
                    x.Data.Size,
                    Description = $"{x.Data.Size} Fries"
                });
            }
        }
    }
}
