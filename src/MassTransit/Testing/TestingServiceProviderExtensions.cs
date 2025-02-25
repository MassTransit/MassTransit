#nullable enable
namespace MassTransit.Testing
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Saga;


    public static class TestingServiceProviderExtensions
    {
        public static ITestHarness GetTestHarness(this IServiceProvider provider)
        {
            return provider.GetRequiredService<ITestHarness>();
        }

        public static async Task<ITestHarness> StartTestHarness(this IServiceProvider provider)
        {
            var testHarness = provider.GetRequiredService<ITestHarness>();

            await testHarness.Start().ConfigureAwait(false);

            return testHarness;
        }

        public static async Task<Task<ConsumeContext<T>>> ConnectPublishHandler<T>(this ITestHarness harness, Func<ConsumeContext<T>, bool> filter)
            where T : class
        {
            TaskCompletionSource<ConsumeContext<T>> source = harness.GetTask<ConsumeContext<T>>();

            var handle = harness.Bus.ConnectReceiveEndpoint(configurator =>
            {
                configurator.Handler<T>(async context =>
                {
                    if (filter(context))
                        source.TrySetResult(context);
                });
            });

            await handle.Ready;

            return source.Task;
        }

        public static void AddTaskCompletionSource<T>(this IBusRegistrationConfigurator configurator)
        {
            configurator.AddSingleton(provider => provider.GetRequiredService<ITestHarness>().GetTask<T>());
        }

        /// <summary>
        /// Stop the test harness, which stops the bus and all hosted services that were started.
        /// </summary>
        /// <param name="harness"></param>
        /// <param name="cancellationToken"></param>
        public static async Task Stop(this ITestHarness harness, CancellationToken cancellationToken = default)
        {
            IHostedService[] services = harness.Provider.GetServices<IHostedService>().ToArray();

            foreach (var service in services.Reverse())
                await service.StopAsync(cancellationToken).ConfigureAwait(false);
        }

        public static async Task RestartHostedServices(this ITestHarness harness, CancellationToken cancellationToken = default)
        {
            IHostedService[] services = harness.Provider.GetServices<IHostedService>().ToArray();

            foreach (var service in services.Reverse())
                await service.StopAsync(cancellationToken).ConfigureAwait(false);

            foreach (var service in services)
                await service.StartAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Adds a saga instance to the in-memory saga repository.
        /// </summary>
        /// <param name="harness">The test harness</param>
        /// <param name="correlationId">The correlationId for the newly created saga instance</param>
        /// <param name="callback">Callback to set any additional properties on the saga instance</param>
        /// <typeparam name="T">The saga type</typeparam>
        public static void AddSagaInstance<T>(this ITestHarness harness, Guid? correlationId = default, Action<T>? callback = null)
            where T : class, ISaga, new()
        {
            var dictionary = harness.Provider.GetService<IndexedSagaDictionary<T>>();
            if (dictionary == null)
                throw new ArgumentException("In-memory saga repository not found", nameof(T));

            if (correlationId.HasValue && dictionary[correlationId.Value] != null)
                throw new ArgumentException($"An existing saga instance with the specified correlationId was found: {correlationId}", nameof(correlationId));

            var instance = new T { CorrelationId = correlationId ?? NewId.NextGuid() };
            callback?.Invoke(instance);

            dictionary.Add(new SagaInstance<T>(instance));
        }

        /// <summary>
        /// Adds or updates an existing saga instance using the in-memory saga repository.
        /// </summary>
        /// <param name="harness"></param>
        /// <param name="correlationId"></param>
        /// <param name="callback"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        public static async Task AddOrUpdateSagaInstance<T>(this ITestHarness harness, Guid? correlationId = default, Action<T>? callback = null,
            CancellationToken cancellationToken = default)
            where T : class, ISaga, new()
        {
            correlationId ??= NewId.NextGuid();

            var dictionary = harness.Provider.GetService<IndexedSagaDictionary<T>>();
            if (dictionary == null)
                throw new ArgumentException("In-memory saga repository not found", nameof(T));

            await dictionary.MarkInUse(cancellationToken).ConfigureAwait(false);
            try
            {
                SagaInstance<T> existingSaga = dictionary[correlationId.Value];
                if (existingSaga != null)
                {
                    await existingSaga.MarkInUse(cancellationToken).ConfigureAwait(false);

                    existingSaga.Remove();
                    dictionary.Remove(existingSaga);
                }

                var instance = new T { CorrelationId = correlationId.Value };
                callback?.Invoke(instance);

                dictionary.Add(new SagaInstance<T>(instance));
            }
            finally
            {
                dictionary.Release();
            }
        }

        /// <summary>
        /// Removes a saga instance from the in-memory saga repository (if it exists).
        /// </summary>
        /// <param name="harness"></param>
        /// <param name="correlationId"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static async Task<bool> TryRemoveSagaInstance<T>(this ITestHarness harness, Guid correlationId, CancellationToken cancellationToken = default)
            where T : class, ISaga
        {
            var dictionary = harness.Provider.GetService<IndexedSagaDictionary<T>>();
            if (dictionary == null)
                throw new ArgumentException("In-memory saga repository not found", nameof(T));

            await dictionary.MarkInUse(cancellationToken).ConfigureAwait(false);
            try
            {
                SagaInstance<T> existingSaga = dictionary[correlationId];
                if (existingSaga != null)
                {
                    await existingSaga.MarkInUse(cancellationToken).ConfigureAwait(false);

                    existingSaga.Remove();
                    dictionary.Remove(existingSaga);

                    return true;
                }
            }
            finally
            {
                dictionary.Release();
            }

            return false;
        }
    }
}
