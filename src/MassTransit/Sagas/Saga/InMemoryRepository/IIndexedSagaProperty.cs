namespace MassTransit.Saga
{
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// For the in-memory saga repository, this maintains an index of saga properties
    /// for fast searching
    /// </summary>
    /// <typeparam name="TSaga">The saga type</typeparam>
    public interface IIndexedSagaProperty<TSaga>
        where TSaga : class, ISaga
    {
        /// <summary>
        /// Returns the saga with the specified key
        /// </summary>
        /// <param name="key"></param>
        SagaInstance<TSaga> this[object key] { get; }

        int Count { get; }

        /// <summary>
        /// Adds a new saga to the index
        /// </summary>
        /// <param name="newItem"></param>
        void Add(SagaInstance<TSaga> newItem);

        /// <summary>
        /// Removes a saga from the index
        /// </summary>
        /// <param name="item"></param>
        void Remove(SagaInstance<TSaga> item);

        /// <summary>
        /// Returns sagas matching the filter function
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IEnumerable<SagaInstance<TSaga>> Where(Func<TSaga, bool> filter);

        /// <summary>
        /// Returns sagas matching the filter function where the key also matches
        /// </summary>
        /// <param name="key"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        IEnumerable<SagaInstance<TSaga>> Where(object key, Func<TSaga, bool> filter);

        /// <summary>
        /// Selects sagas from the index, running the transformation function and returning the output type
        /// </summary>
        /// <param name="transformer"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        IEnumerable<TResult> Select<TResult>(Func<TSaga, TResult> transformer);
    }
}
