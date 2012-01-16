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
namespace MassTransit
{
    /// <summary>
	/// Implementations can be added to the inbound message pipeline to intercept
	/// messages before they are delivered to any consumers.
	/// </summary>
	public interface IInboundMessageInterceptor
	{
		/// <summary>
		/// Called before the message is dispatched to the next handler in the pipeline
		/// </summary>
		/// <param name="context">The context of the message being dispatched</param>
		void PreDispatch(IConsumeContext context);

		/// <summary>
		/// Calls after the message has been dispatched through the pipeline
		/// </summary>
		/// <param name="context">The context of the message being dispatched</param>
		void PostDispatch(IConsumeContext context);
	}
}