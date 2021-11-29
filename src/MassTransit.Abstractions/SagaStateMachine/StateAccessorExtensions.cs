namespace MassTransit
{
    using System.Threading.Tasks;


    public static class StateAccessorExtensions
    {
        public static Task<State<TSaga>> GetState<TSaga>(this IStateAccessor<TSaga> accessor, BehaviorContext<TSaga> context)
            where TSaga : class, ISaga
        {
            return accessor.Get(context);
        }

        public static Task<State<TSaga>> GetState<TSaga>(this StateMachine<TSaga> accessor, BehaviorContext<TSaga> context)
            where TSaga : class, ISaga
        {
            return accessor.Accessor.Get(context);
        }
    }
}
