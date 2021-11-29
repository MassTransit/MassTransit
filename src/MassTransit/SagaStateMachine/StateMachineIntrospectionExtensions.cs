namespace MassTransit
{
    using System.Collections.Generic;
    using System.Threading.Tasks;


    public static class StateMachineIntrospectionExtensions
    {
        public static async Task<IEnumerable<Event>> NextEvents<TInstance>(this BehaviorContext<TInstance> context)
            where TInstance : class, ISaga
        {
            return context.StateMachine.NextEvents(await context.StateMachine.Accessor.Get(context));
        }
    }
}
