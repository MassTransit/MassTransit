// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Saga
{
	using System;
	using System.Collections.Generic;
	using MassTransit.Pipeline;

	/// <summary>
	/// A saga repository is used by the service bus to dispatch messages to sagas
	/// </summary>
	/// <typeparam name="TSaga"></typeparam>
	public interface ISagaRepository<TSaga>
		where TSaga : class, ISaga
	{
        /// <summary>
        /// Loads/Creates the saga and makes it available for later consumption
        /// through the Actions
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="context"></param>
        /// <param name="sagaId"></param>
        /// <param name="selector"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
		IEnumerable<Action<IConsumeContext<TMessage>>> GetSaga<TMessage>(IConsumeContext<TMessage> context, Guid sagaId,
																		 InstanceHandlerSelector<TSaga, TMessage> selector,
																		 ISagaPolicy<TSaga, TMessage> policy)
			where TMessage : class;


        /// <summary>
        /// Finds the CorrelationIds for the sagas that match the filter
        /// </summary>
        /// <param name="filter">effectively a LINQ expression</param>
        /// <returns></returns>
		IEnumerable<Guid> Find(ISagaFilter<TSaga> filter);

        /// <summary>
        /// Finds the sagas that match the filter
        /// </summary>
        /// <param name="filter">effectively a LINQ expression</param>
        /// <returns></returns>
		IEnumerable<TSaga> Where(ISagaFilter<TSaga> filter);

        /// <summary>
        /// Finds all of the sagas that match the filter, and then applies a transform on them.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="filter">effectively a LINQ expression</param>
        /// <param name="transformer"></param>
        /// <returns></returns>
		IEnumerable<TResult> Where<TResult>(ISagaFilter<TSaga> filter, Func<TSaga, TResult> transformer);


        /// <summary>
        /// Queries the underlying store for sagas, then applies a transform to them and
        /// returns the result
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="transformer"></param>
        /// <returns></returns>
		IEnumerable<TResult> Select<TResult>(Func<TSaga, TResult> transformer);
	}
}