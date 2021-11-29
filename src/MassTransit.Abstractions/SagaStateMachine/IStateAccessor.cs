namespace MassTransit
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;


    public interface IStateAccessor<TSaga> :
        IProbeSite
        where TSaga : class, ISaga
    {
        Task<State<TSaga>> Get(BehaviorContext<TSaga> context);

        Task Set(BehaviorContext<TSaga> context, State<TSaga> state);

        /// <summary>
        /// Converts a state expression to the instance current state property type.
        /// </summary>
        /// <param name="states"></param>
        /// <returns></returns>
        Expression<Func<TSaga, bool>> GetStateExpression(params State[] states);
    }
}
