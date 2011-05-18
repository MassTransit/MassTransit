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
	/// <summary>
	/// A saga policy defines how the pipeline should handle messages when being routed 
	/// to the saga. Checks are made for the existence of a saga, whether the message should
	/// create a new saga, or otherwise
	/// </summary>
	/// <typeparam name="TSaga">The saga that will handle the message</typeparam>
	/// <typeparam name="TMessage">The message that will be handled by the saga</typeparam>
	public interface ISagaPolicy<TSaga, TMessage>
		where TSaga : class, ISaga
	{
		/// <summary>
		/// Determines if the message should result in the creation of a new instance of the saga
		/// </summary>
		/// <param name="message">The message that triggered the action</param>
		/// <param name="saga">The saga that was created</param>
		/// <returns>True if the saga should be created, otherwise false</returns>
		bool CreateSagaWhenMissing(TMessage message, out TSaga saga);

		/// <summary>
		/// Called when an instance of the saga associated with the message already exists
		/// </summary>
		/// <param name="message">The message to correlate to the saga</param>
		void ForExistingSaga(TMessage message);

		/// <summary>
		/// Called when there are no matching instances of the saga available
		/// </summary>
		/// <param name="message">The message to correlate to the saga</param>
		void ForMissingSaga(TMessage message);

		/// <summary>
		/// Checks to see if the saga should be removed based on the current state
		/// </summary>
		/// <param name="saga"></param>
		/// <returns></returns>
		bool ShouldSagaBeRemoved(TSaga saga);
	}
}