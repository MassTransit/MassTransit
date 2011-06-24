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
	using System;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// Returns zero to many handlers for the message, aligns with the pipeline sink requirements
	/// </summary>
	/// <typeparam name="TMessage"></typeparam>
	/// <param name="context"></param>
	/// <returns></returns>
	public delegate IEnumerable<Action<IConsumeContext<TMessage>>> MultipleHandlerSelector<TMessage>(
		IConsumeContext<TMessage> context)
		where TMessage : class;

	/// <summary>
	/// A static class to create delegates easily
	/// </summary>
	public static class MultipleHandlerSelector
	{
		/// <summary>
		/// Creates a MultipleHandlerSelector for the selector 
		/// </summary>
		/// <typeparam name="TMessage">The message type of the handler</typeparam>
		/// <param name="selector">The selector to promote to a MultipleHandlerSelector</param>
		/// <returns>A MultipleHandlerSelector delegate for the specified message type</returns>
		public static MultipleHandlerSelector<TMessage> ForHandler<TMessage>(HandlerSelector<TMessage> selector)
			where TMessage : class
		{
			return context =>
				{
					Action<IConsumeContext<TMessage>> handler = selector(context);
					if (handler == null)
					{
						return Enumerable.Empty<Action<IConsumeContext<TMessage>>>();
					}

					return Enumerable.Repeat(handler, 1);
				};
		}
	}
}