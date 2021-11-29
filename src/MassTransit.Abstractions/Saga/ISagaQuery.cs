namespace MassTransit
{
    using System;
    using System.Linq.Expressions;


    /// <summary>
    /// A saga query is used when a LINQ expression is accepted to query
    /// the saga repository storage to get zero or more saga instances
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public interface ISagaQuery<TSaga>
        where TSaga : class, ISaga
    {
        /// <summary>
        /// The query expression that returns true if the saga
        /// matches the query.
        /// </summary>
        Expression<Func<TSaga, bool>> FilterExpression { get; }

        /// <summary>
        /// Compiles a function that can be used to programatically
        /// compare a saga instance to the filter expression.
        /// </summary>
        /// <returns></returns>
        Func<TSaga, bool> GetFilter();
    }
}
