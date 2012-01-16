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
	/// Implementers should manage subscriptions from the subscription events,
	/// see also <see cref="ISubscriptionEvent"/> - which is passed to implementers of this
	/// interface.
	/// </summary>
	public interface IInboundPipelineConfigurator :
		ISubscriptionEvent
	{
		/// <summary>
		/// Gets the inbound message pipeline.
		/// </summary>
		IInboundMessagePipeline Pipeline { get; }
		
		/// <summary>
		/// Gets the service bus under configuration.
		/// </summary>
		IServiceBus Bus { get; }

		/// <summary>
		/// Register some instance that cares about message-subscriptions.
		/// </summary>
		/// <param name="subscriptionEventHandler">Instance</param>
		/// <returns>An unsubscribing multi-cast delegate.</returns>
		UnregisterAction Register(ISubscriptionEvent subscriptionEventHandler);
	}
}