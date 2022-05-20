namespace MassTransit.Testing
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;


    public interface ITestHarness :
        IBaseTestHarness
    {
        IBus Bus { get; }

        IServiceScope Scope { get; }

        IEndpointNameFormatter EndpointNameFormatter { get; }

        /// <summary>
        /// Returns a task completion source that is automatically canceled when the test is canceled
        /// </summary>
        /// <typeparam name="T">The task type</typeparam>
        /// <returns></returns>
        TaskCompletionSource<T> GetTask<T>();

        /// <summary>
        /// Gets the consumer harness for the specified consumer from the container. Consumer test
        /// harnesses are automatically added to the container when AddConsumer is used.
        /// </summary>
        /// <typeparam name="T">The consumer type</typeparam>
        /// <returns></returns>
        IConsumerTestHarness<T> GetConsumerHarness<T>()
            where T : class, IConsumer;

        /// <summary>
        /// Gets the saga harness for the specified saga from the container. Saga test
        /// harnesses are automatically added to the container when AddSaga is used.
        /// </summary>
        /// <typeparam name="T">The saga type</typeparam>
        /// <returns></returns>
        ISagaTestHarness<T> GetSagaHarness<T>()
            where T : class, ISaga;

        /// <summary>
        /// Gets the saga harness for the specified saga from the container. Saga test
        /// harnesses are automatically added to the container when AddSaga is used.
        /// </summary>
        /// <typeparam name="T">The saga type</typeparam>
        /// <typeparam name="TStateMachine">The state machine type</typeparam>
        /// <returns></returns>
        ISagaStateMachineTestHarness<TStateMachine, T> GetSagaStateMachineHarness<TStateMachine, T>()
            where TStateMachine : class, SagaStateMachine<T>
            where T : class, SagaStateMachineInstance;

        IRequestClient<T> GetRequestClient<T>()
            where T : class;

        /// <summary>
        /// Use the endpoint name formatter to get the send endpoint for the consumer type
        /// </summary>
        /// <typeparam name="T">The consumer type</typeparam>
        /// <returns></returns>
        Task<ISendEndpoint> GetConsumerEndpoint<T>()
            where T : class, IConsumer;

        /// <summary>
        /// Use the endpoint name formatter to get the send endpoint for the saga type
        /// </summary>
        /// <typeparam name="T">The saga type</typeparam>
        /// <returns></returns>
        Task<ISendEndpoint> GetSagaEndpoint<T>()
            where T : class, ISaga;

        /// <summary>
        /// Use the endpoint name formatter to get the execute send endpoint for the activity type
        /// </summary>
        /// <typeparam name="T">The activity type</typeparam>
        /// <typeparam name="TArguments">The argument type</typeparam>
        /// <returns></returns>
        Task<ISendEndpoint> GetExecuteActivityEndpoint<T, TArguments>()
            where T : class, IExecuteActivity<TArguments>
            where TArguments : class;

        Task Start();
    }
}
