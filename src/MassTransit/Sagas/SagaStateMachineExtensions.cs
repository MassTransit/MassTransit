namespace MassTransit
{
    using System;
    using System.Linq.Expressions;
    using Internals;
    using Saga;


    public static class SagaStateMachineExtensions
    {
        /// <summary>
        /// Create a query that combines the specified expression with an expression that compares the instance state with the specified states
        /// </summary>
        /// <param name="machine">The state machine</param>
        /// <param name="expression">The query expression</param>
        /// <param name="states">The states that are valid for this query</param>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <returns></returns>
        public static ISagaQuery<TInstance> CreateSagaQuery<TInstance>(this StateMachine<TInstance> machine, Expression<Func<TInstance, bool>> expression,
            params State[] states)
            where TInstance : class, ISaga
        {
            Expression<Func<TInstance, bool>> stateExpression = machine.Accessor.GetStateExpression(states);

            return new SagaQuery<TInstance>(StateExpressionVisitor<TInstance>.Combine(expression, stateExpression));
        }

        /// <summary>
        /// Create a query that combines the specified expression with an expression that compares the instance state with the specified states
        /// </summary>
        /// <param name="machine">The state machine</param>
        /// <param name="expression">The query expression</param>
        /// <param name="states">The states that are valid for this query</param>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <returns></returns>
        public static Func<TInstance, bool> CreateSagaFilter<TInstance>(this StateMachine<TInstance> machine, Expression<Func<TInstance, bool>> expression,
            params State[] states)
            where TInstance : class, ISaga
        {
            Expression<Func<TInstance, bool>> stateExpression = machine.Accessor.GetStateExpression(states);

            return StateExpressionVisitor<TInstance>.Combine(expression, stateExpression).CompileFast();
        }
    }
}
