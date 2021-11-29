namespace MassTransit.SagaStateMachine
{
    using System.Threading.Tasks;
    using Util;


    public class StateObservable<TSaga> :
        Connectable<IStateObserver<TSaga>>,
        IStateObserver<TSaga>
        where TSaga : class, ISaga
    {
        public Task StateChanged(BehaviorContext<TSaga> context, State currentState, State previousState)
        {
            return ForEachAsync(x => x.StateChanged(context, currentState, previousState));
        }
    }
}
