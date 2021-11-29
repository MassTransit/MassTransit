namespace MassTransit.Testing
{
    using System.Threading.Tasks;


    public interface ITestHarness :
        IBaseTestHarness
    {
        IBus Bus { get; }

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

        Task Start();
    }
}
