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

	/// <summary>
	/// Given a message, returns the handler for that message, or null if the message
	/// should be ignored.
	/// </summary>
	/// <typeparam name="TMessage">The message type</typeparam>
	/// <param name="context">The consume context of the message</param>
	/// <returns></returns>
	public delegate Action<IConsumeContext<TMessage>> HandlerSelector<TMessage>(IConsumeContext<TMessage> context)
		where TMessage : class;

	/// <summary>
	/// Helpers for creating HandlerSelectors
	/// </summary>
	public static class HandlerSelector
	{
		public static HandlerSelector<TMessage> ForHandler<TMessage>(Action<TMessage> handler)
			where TMessage : class
		{
			return context => x =>
				{
					using (x.CreateScope())
					{
						handler(x.Message);
					}
				};
		}

		public static HandlerSelector<TMessage> ForCondition<TMessage>(HandlerSelector<TMessage> handler, Predicate<TMessage> condition) 
			where TMessage : class
		{
			return context =>
			{
				using (context.CreateScope())
				{
					if (!condition(context.Message))
						return null;
				}

				return handler(context);
			};
		}

		public static HandlerSelector<TMessage> ForContextHandler<TMessage>(Action<IConsumeContext<TMessage>> handler)
			where TMessage : class
		{
			return context => handler;
		}

		public static HandlerSelector<TMessage> ForSelectiveHandler<TMessage>(Predicate<TMessage> condition,
		                                                                      Action<TMessage> handler)
			where TMessage : class
		{
			return context =>
				{
					if (condition != null)
					{
						using (context.CreateScope())
						{
							if (!condition(context.Message))
								return null;
						}
					}

					return x =>
						{
							using (x.CreateScope())
							{
								handler(x.Message);
							}
						};
				};
		}
	}
}