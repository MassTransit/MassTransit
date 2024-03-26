namespace MassTransit
{
    using System.Threading.Tasks;
    using Util;


    public partial class MassTransitStateMachine<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        public class StateObservable :
            Connectable<IStateObserver<TInstance>>,
            IStateObserver<TInstance>
        {
            public Task StateChanged(BehaviorContext<TInstance> context, State currentState, State previousState)
            {
                return ForEachAsync(x => x.StateChanged(context, currentState, previousState));
            }

            public void Method4()
            {
            }

            public void Method5()
            {
            }

            public void Method6()
            {
            }
        }
    }
}
