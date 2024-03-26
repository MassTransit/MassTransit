namespace MassTransit
{
    /// <summary>
    /// Return the name of the event hub
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate string EventHubNameProvider<TInstance>(BehaviorContext<TInstance> context)
        where TInstance : class, SagaStateMachineInstance;


    /// <summary>
    /// Return the name of the event hub
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate string EventHubNameProvider<TInstance, in TMessage>(BehaviorContext<TInstance, TMessage> context)
        where TInstance : class, SagaStateMachineInstance
        where TMessage : class;
}
