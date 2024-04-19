namespace MassTransit.Tests.ContainerTests.Common_Tests
{
    using System;
    using System.Threading.Tasks;
    using Mediator;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Scenarios;
    using TestFramework;


    public class Common_ScopedMediator_Send :
        InMemoryContainerTestFixture
    {
        readonly TaskCompletionSource<MyId> _taskCompletionSource;

        public Common_ScopedMediator_Send()
        {
            _taskCompletionSource = GetTask<MyId>();
        }

        MyId MyId => ServiceScope.ServiceProvider.GetRequiredService<MyId>();

        IScopedMediator Mediator => ServiceScope.ServiceProvider.GetRequiredService<IScopedMediator>();

        [Test]
        public async Task Should_use_scope()
        {
            await Mediator.Send<SimpleMessageInterface>(new { Name = "test" });

            var result = await _taskCompletionSource.Task;
            Assert.That(result, Is.EqualTo(MyId));
        }

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            collection.AddScoped(_ => new MyId(Guid.NewGuid()));
            collection.AddSingleton(_taskCompletionSource);
            return collection.AddMediator(ConfigureRegistration);
        }

        void ConfigureRegistration(IMediatorRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<SimpleScopedMediatorConsumer>();
            configurator.ConfigureMediator(ConfigureMediator);
        }

        static void ConfigureMediator(IMediatorRegistrationContext context, IMediatorConfigurator configurator)
        {
        }
    }


    public class Common_ScopedMediator_Publish :
        InMemoryContainerTestFixture
    {
        readonly TaskCompletionSource<MyId> _taskCompletionSource;

        public Common_ScopedMediator_Publish()
        {
            _taskCompletionSource = GetTask<MyId>();
        }

        MyId MyId => ServiceScope.ServiceProvider.GetRequiredService<MyId>();

        IScopedMediator Mediator => ServiceScope.ServiceProvider.GetRequiredService<IScopedMediator>();

        [Test]
        public async Task Should_use_scope()
        {
            await Mediator.Publish<SimpleMessageInterface>(new { Name = "test" });

            var result = await _taskCompletionSource.Task;
            Assert.That(result, Is.EqualTo(MyId));
        }

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            collection.AddScoped(_ => new MyId(Guid.NewGuid()));
            collection.AddSingleton(_taskCompletionSource);
            return collection.AddMediator(ConfigureRegistration);
        }

        static void ConfigureRegistration(IMediatorRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<SimpleScopedMediatorConsumer>();
            configurator.ConfigureMediator(ConfigureMediator);
        }

        static void ConfigureMediator(IMediatorRegistrationContext context, IMediatorConfigurator configurator)
        {
        }
    }


    public class Common_ScopedMediator_Consecutive_Send :
        InMemoryContainerTestFixture
    {
        readonly TaskCompletionSource<MyOtherId> _otherTaskCompletionSource;
        readonly TaskCompletionSource<MyId> _taskCompletionSource;

        public Common_ScopedMediator_Consecutive_Send()
        {
            _taskCompletionSource = GetTask<MyId>();
            _otherTaskCompletionSource = GetTask<MyOtherId>();
        }

        MyId MyId => ServiceScope.ServiceProvider.GetRequiredService<MyId>();
        MyOtherId MyOtherId => ServiceScope.ServiceProvider.GetRequiredService<MyOtherId>();

        IScopedMediator Mediator => ServiceScope.ServiceProvider.GetRequiredService<IScopedMediator>();

        [Test]
        public async Task Should_use_scope()
        {
            await Mediator.Send<SimplePublishedInterface>(new { Name = "test" });

            var myOtherId = await _otherTaskCompletionSource.Task;
            Assert.That(myOtherId, Is.EqualTo(MyOtherId));

            var myId = await _taskCompletionSource.Task;
            Assert.That(myId, Is.EqualTo(MyId));
        }

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            collection.AddScoped(_ => new MyId(Guid.NewGuid()));
            collection.AddScoped(_ => new MyOtherId(Guid.NewGuid()));
            collection.AddSingleton(_taskCompletionSource);
            collection.AddSingleton(_otherTaskCompletionSource);
            return collection.AddMediator(ConfigureRegistration);
        }

        static void ConfigureRegistration(IMediatorRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<ConsecutiveScopedMediatorConsumer>();
            configurator.AddConsumer<SimpleScopedMediatorConsumer>();
            configurator.ConfigureMediator(ConfigureMediator);
        }

        static void ConfigureMediator(IMediatorRegistrationContext context, IMediatorConfigurator configurator)
        {
        }
    }


    public class Common_ScopedMediator_Consecutive_Publish :
        InMemoryContainerTestFixture
    {
        readonly TaskCompletionSource<MyOtherId> _otherTaskCompletionSource;
        readonly TaskCompletionSource<MyId> _taskCompletionSource;

        public Common_ScopedMediator_Consecutive_Publish()
        {
            _taskCompletionSource = GetTask<MyId>();
            _otherTaskCompletionSource = GetTask<MyOtherId>();
        }

        MyId MyId => ServiceScope.ServiceProvider.GetRequiredService<MyId>();
        MyOtherId MyOtherId => ServiceScope.ServiceProvider.GetRequiredService<MyOtherId>();

        IScopedMediator Mediator => ServiceScope.ServiceProvider.GetRequiredService<IScopedMediator>();

        [Test]
        public async Task Should_use_scope()
        {
            await Mediator.Publish<SimplePublishedInterface>(new { Name = "test" });

            var myOtherId = await _otherTaskCompletionSource.Task;
            Assert.That(myOtherId, Is.EqualTo(MyOtherId));

            var myId = await _taskCompletionSource.Task;
            Assert.That(myId, Is.EqualTo(MyId));
        }

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            collection.AddScoped(_ => new MyId(Guid.NewGuid()));
            collection.AddScoped(_ => new MyOtherId(Guid.NewGuid()));
            collection.AddSingleton(_taskCompletionSource);
            collection.AddSingleton(_otherTaskCompletionSource);
            return collection.AddMediator(ConfigureRegistration);
        }

        static void ConfigureRegistration(IMediatorRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<ConsecutiveScopedMediatorConsumer>();
            configurator.AddConsumer<SimpleScopedMediatorConsumer>();
            configurator.ConfigureMediator(ConfigureMediator);
        }

        static void ConfigureMediator(IMediatorRegistrationContext context, IMediatorConfigurator configurator)
        {
        }
    }


    public class Common_ScopedMediator_RequestResponse :
        InMemoryContainerTestFixture
    {
        readonly TaskCompletionSource<MyId> _taskCompletionSource;

        public Common_ScopedMediator_RequestResponse()
        {
            _taskCompletionSource = GetTask<MyId>();
        }

        MyId MyId => ServiceScope.ServiceProvider.GetRequiredService<MyId>();

        [Test]
        public async Task Should_use_scope()
        {
            IRequestClient<SimpleMessageInterface> client = GetRequestClient<SimpleMessageInterface>();
            await client.GetResponse<ISimpleMessageResponse>(new { Name = "test" });

            var result = await _taskCompletionSource.Task;
            Assert.That(result, Is.EqualTo(MyId));
        }

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            collection.AddScoped(_ => new MyId(Guid.NewGuid()));
            collection.AddSingleton(_taskCompletionSource);
            return collection.AddMediator(ConfigureRegistration);
        }

        static void ConfigureRegistration(IMediatorRegistrationConfigurator configurator)
        {
            configurator.AddRequestClient<SimpleMessageInterface>();
            configurator.AddConsumer<SimpleScopedMediatorConsumer>();
            configurator.ConfigureMediator(ConfigureMediator);
        }

        static void ConfigureMediator(IMediatorRegistrationContext context, IMediatorConfigurator configurator)
        {
        }
    }


    class SimpleScopedMediatorConsumer :
        IConsumer<SimpleMessageInterface>
    {
        readonly MyId _myId;
        readonly TaskCompletionSource<MyId> _taskCompletionSource;

        public SimpleScopedMediatorConsumer(TaskCompletionSource<MyId> taskCompletionSource, MyId myId)
        {
            _taskCompletionSource = taskCompletionSource;
            _myId = myId;
        }

        public Task Consume(ConsumeContext<SimpleMessageInterface> context)
        {
            _taskCompletionSource.TrySetResult(_myId);
            return context.ResponseAddress != null ? context.RespondAsync<ISimpleMessageResponse>(new { }) : Task.CompletedTask;
        }
    }


    class ConsecutiveScopedMediatorConsumer :
        IConsumer<SimplePublishedInterface>
    {
        readonly IScopedMediator _mediator;
        readonly MyOtherId _otherId;
        readonly TaskCompletionSource<MyOtherId> _taskCompletionSource;

        public ConsecutiveScopedMediatorConsumer(TaskCompletionSource<MyOtherId> taskCompletionSource, IScopedMediator mediator, MyOtherId otherId)
        {
            _taskCompletionSource = taskCompletionSource;
            _mediator = mediator;
            _otherId = otherId;
        }

        public Task Consume(ConsumeContext<SimplePublishedInterface> context)
        {
            _taskCompletionSource.TrySetResult(_otherId);
            return _mediator.Send<SimpleMessageInterface>(new { Name = "test" });
        }
    }


    class MyOtherId :
        MyId
    {
        public MyOtherId(Guid id)
            : base(id)
        {
        }
    }
}
