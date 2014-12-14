// Copyright 2007-2008 The Apache Software Foundation.
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

	/// <summary>
	/// A saga policy defines how the pipeline should handle messages when being routed 
	/// to the saga. Checks are made for the existence of a saga, whether the message should
	/// create a new saga, or otherwise
	/// </summary>
	/// <typeparam name="TSaga">The saga that will handle the message</typeparam>
	/// <typeparam name="TMessage">The message that will be handled by the saga</typeparam>
	public interface ISagaPolicy<TSaga, in TMessage>
		where TSaga : class, ISaga
		where TMessage : class
	{
		/// <summary>
		/// Determines if the message is able to create a new instance of the saga
		/// </summary>
		/// <param name="context">The consumer context of the message</param>
		/// <returns>True if a new instance of the saga can be created, otherwise false</returns>
		bool CanCreateInstance(ConsumeContext<TMessage> context);

		/// <summary>
		/// Creates a new instance of the saga using the data in the message context
		/// </summary>
		/// <param name="context">The consumer context of the message</param>
		/// <param name="sagaId"></param>
		/// <returns>A newly created saga instance</returns>
		TSaga CreateInstance(ConsumeContext<TMessage> context, Guid sagaId);

		/// <summary>
		/// Returns the saga id that should be used for a newly created saga instance, based on the policy
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		Guid GetNewSagaId(ConsumeContext<TMessage> context);

		/// <summary>
		/// Determines if the message can be delivered to an existing saga instance
		/// </summary>
		/// <param name="context">The consumer context of the message</param>
		/// <returns>True if the message can be delivered to the saga instance, otherwise false</returns>
		bool CanUseExistingInstance(ConsumeContext<TMessage> context);


		/// <summary>
		/// Determines if the saga instance can be removed, using the saga configuration information
		/// </summary>
		/// <param name="instance">The saga instance to check</param>
		/// <returns>True if the saga instance can be removed, otherwise false</returns>
		bool CanRemoveInstance(TSaga instance);
	}
}