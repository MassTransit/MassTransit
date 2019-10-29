namespace MassTransit.Containers.Tests
{
    using System.Threading.Tasks;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using GreenPipes.Internals.Extensions;
    using NUnit.Framework;
    using Scenarios;
    using TestFramework;
    using Util;
    using WindsorIntegration;


    public class Using_message_scope_with_two_consumers :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_a_message_in_scope()
        {
            const string name = "Joe";

            await InputQueueSendEndpoint.Send(new SimpleMessageClass(name));

            var result = await Dependency.Completed.OrCanceled(TestCancellationToken);
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        readonly IWindsorContainer _container;

        public Using_message_scope_with_two_consumers()
        {
            _container = new WindsorContainer();
            _container.Register(
                Component.For<FirstConsumer>(),
                Component.For<SecondConsumer>(),
                Component.For<IScopedDependency>()
                    .ImplementedBy<Dependency>()
                    .LifestyleScoped<MessageScope>());
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseMessageScope();

            configurator.Consumer<FirstConsumer>(_container);
            configurator.Consumer<SecondConsumer>(_container);
        }


        public class FirstConsumer :
            IConsumer<SimpleMessageInterface>
        {
            readonly IScopedDependency _dependency;

            public FirstConsumer(IScopedDependency dependency)
            {
                _dependency = dependency;
            }

            public async Task Consume(ConsumeContext<SimpleMessageInterface> context)
            {
                _dependency.CompleteFirst();
            }
        }


        public class SecondConsumer :
            IConsumer<SimpleMessageInterface>
        {
            readonly IScopedDependency _dependency;

            public SecondConsumer(IScopedDependency dependency)
            {
                _dependency = dependency;
            }

            public async Task Consume(ConsumeContext<SimpleMessageInterface> context)
            {
                _dependency.CompleteSecond();
            }
        }


        public class Dependency :
            IScopedDependency
        {
            static readonly TaskCompletionSource<string> _completed;
            readonly TaskCompletionSource<string> _first;
            readonly TaskCompletionSource<string> _second;

            static Dependency()
            {
                _completed = TaskUtil.GetTask<string>();
            }

            public Dependency()
            {
                _first = TaskUtil.GetTask<string>();
                _second = TaskUtil.GetTask<string>();
            }

            public static Task<string> Completed => _completed.Task;

            public void CompleteFirst()
            {
                lock (_completed)
                {
                    _first.TrySetResult("First");

                    if (_second.Task.Status == TaskStatus.RanToCompletion)
                        _completed.TrySetResult(_second.Task.Result);
                }
            }

            public void CompleteSecond()
            {
                lock (_completed)
                {
                    _second.TrySetResult("Second");

                    if (_first.Task.Status == TaskStatus.RanToCompletion)
                        _completed.TrySetResult(_first.Task.Result);
                }
            }
        }


        public interface IScopedDependency
        {
            void CompleteFirst();
            void CompleteSecond();
        }
    }
}
