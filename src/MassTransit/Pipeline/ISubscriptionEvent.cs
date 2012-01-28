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
namespace MassTransit.Pipeline
{
	/// <summary>
	/// Notifies when changes to the subscribed message types changes in the pipeline.
	/// </summary>
	public interface ISubscriptionEvent
	{
		/// <summary>
		/// Notify of a subscription for a message.
		/// </summary>
		/// <typeparam name="TMessage">Message type</typeparam>
		/// <returns>The corresponding action for unsubscription</returns>
		UnsubscribeAction SubscribedTo<TMessage>()
			where TMessage : class;

		/// <typeparam name="TMessage">Message type</typeparam>
		/// <typeparam name="TKey">Type of correlation key </typeparam>
		/// <returns>The corresponding action for unsubscription</returns>
		UnsubscribeAction SubscribedTo<TMessage, TKey>(TKey correlationId)
			where TMessage : class, CorrelatedBy<TKey>;
	}
}