namespace MassTransit.Tests.Middleware
{
    using System;
    using System.Threading.Tasks;
    using Contracts;
    using JunkDrawer;
    using MassTransit.Middleware;
    using NUnit.Framework;
    using Util;


    [TestFixture]
    public class Observing_a_dispatch_pipe
    {
        [Test]
        public async Task Should_be_called_post_send()
        {
            IPipeRouter router = new PipeRouter();

            router.ConnectPipe(Pipe.Empty<CommandContext<SetConcurrencyLimit>>());

            var observer = new Observer<CommandContext<SetConcurrencyLimit>>();

            router.ConnectObserver(observer);

            var observer2 = new Observer();

            router.ConnectObserver(observer2);

            await router.SetConcurrencyLimit(32);

            await observer.PostSent;

            Assert.That(async () => await observer2.PostSent, Is.InstanceOf<CommandContext<SetConcurrencyLimit>>());
        }

        [Test]
        public async Task Should_be_called_pre_send()
        {
            IPipeRouter router = new PipeRouter();

            router.ConnectPipe(Pipe.Empty<CommandContext<SetConcurrencyLimit>>());

            var observer = new Observer<CommandContext<SetConcurrencyLimit>>();

            router.ConnectObserver(observer);

            var observer2 = new Observer();

            router.ConnectObserver(observer2);

            await router.SetConcurrencyLimit(32);

            await observer.PreSent;

            Assert.That(async () => await observer2.PreSent, Is.InstanceOf<CommandContext<SetConcurrencyLimit>>());
        }

        [Test]
        public void Should_be_called_when_send_faulted()
        {
            IPipeRouter router = new PipeRouter();

            router.ConnectPipe(Pipe.New<CommandContext<SetConcurrencyLimit>>(cfg =>
            {
                cfg.UseExecute(cxt =>
                {
                    throw new IntentionalTestException("Wow!");
                });
            }));

            var observer = new Observer<CommandContext<SetConcurrencyLimit>>();

            router.ConnectObserver(observer);

            var observer2 = new Observer();

            router.ConnectObserver(observer2);

            Assert.That(async () => await router.SetConcurrencyLimit(32), Throws.TypeOf<IntentionalTestException>());

            Assert.That(async () => await observer.SendFaulted, Throws.TypeOf<IntentionalTestException>());

            Assert.That(async () => await observer2.SendFaulted, Throws.TypeOf<IntentionalTestException>());
        }


        class Observer<TContext> :
            IFilterObserver<TContext>
            where TContext : class, PipeContext
        {
            readonly TaskCompletionSource<TContext> _consumeFaulted;
            readonly TaskCompletionSource<TContext> _postConsumed;
            readonly TaskCompletionSource<TContext> _preConsumed;

            public Observer()
            {
                _preConsumed = TaskUtil.GetTask<TContext>();
                _postConsumed = TaskUtil.GetTask<TContext>();
                _consumeFaulted = TaskUtil.GetTask<TContext>();
            }

            public Task<TContext> PreSent => _preConsumed.Task;
            public Task<TContext> PostSent => _postConsumed.Task;
            public Task<TContext> SendFaulted => _consumeFaulted.Task;

            Task IFilterObserver<TContext>.PreSend(TContext context)
            {
                _preConsumed.TrySetResult(context);

                return Task.CompletedTask;
            }

            Task IFilterObserver<TContext>.PostSend(TContext context)
            {
                _postConsumed.TrySetResult(context);

                return Task.CompletedTask;
            }

            Task IFilterObserver<TContext>.SendFault(TContext context, Exception exception)
            {
                _consumeFaulted.TrySetException(exception);

                return Task.CompletedTask;
            }
        }


        class Observer :
            IFilterObserver
        {
            readonly TaskCompletionSource<CommandContext> _consumeFaulted;
            readonly TaskCompletionSource<CommandContext> _postConsumed;
            readonly TaskCompletionSource<CommandContext> _preConsumed;

            public Observer()
            {
                _preConsumed = TaskUtil.GetTask<CommandContext>();
                _postConsumed = TaskUtil.GetTask<CommandContext>();
                _consumeFaulted = TaskUtil.GetTask<CommandContext>();
            }

            public Task<CommandContext> PreSent => _preConsumed.Task;
            public Task<CommandContext> PostSent => _postConsumed.Task;
            public Task<CommandContext> SendFaulted => _consumeFaulted.Task;

            Task IFilterObserver.PreSend<T>(T context)
            {
                _preConsumed.TrySetResult(context as CommandContext);

                return Task.CompletedTask;
            }

            Task IFilterObserver.PostSend<T>(T context)
            {
                _postConsumed.TrySetResult(context as CommandContext);

                return Task.CompletedTask;
            }

            Task IFilterObserver.SendFault<T>(T context, Exception exception)
            {
                _consumeFaulted.TrySetException(exception);

                return Task.CompletedTask;
            }
        }
    }
}
