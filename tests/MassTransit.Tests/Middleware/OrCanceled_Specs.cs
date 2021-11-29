namespace MassTransit.Tests.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Internals;
    using NUnit.Framework;


    [TestFixture]
    public class When_using_or_canceled_should_not_have_an_unhandled_task_exception
    {
        [Test]
        public async Task Should_fault_on_ready_faulted()
        {
            List<object> unhandledExceptions = new List<object>();
            List<Exception> unobservedTaskExceptions = new List<Exception>();

            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            {
                unhandledExceptions.Add(eventArgs.ExceptionObject);
            };

            TaskScheduler.UnobservedTaskException += (sender, eventArgs) =>
            {
                unobservedTaskExceptions.Add(eventArgs.Exception);
            };

            using (var timeout = new CancellationTokenSource(500))
            {
                async Task DoSomething()
                {
                    await Task.Delay(600);

                    throw new IntentionalTestException("So not ready.");
                }

                var task = DoSomething();

                Assert.That(async () => await task.OrCanceled(timeout.Token), Throws.TypeOf<OperationCanceledException>());
            }

            GC.Collect();
            await Task.Delay(1000);
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Assert.That(unhandledExceptions, Is.Empty);
            Assert.That(unobservedTaskExceptions, Is.Empty);
        }
    }
}
