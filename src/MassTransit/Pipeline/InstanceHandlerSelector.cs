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

	/// <summary>
	/// Given an instance and a message, returns the handlers for the message
	/// </summary>
	/// <typeparam name="TInstance">The instance type</typeparam>
	/// <typeparam name="TMessage">The message type</typeparam>
	/// <param name="instance">The instance to return the handlers for</param>
	/// <returns></returns>
	public delegate Action<TMessage> InstanceHandlerSelector<TInstance, TMessage>(TInstance instance);

	/// <summary>
	/// An instance handler selector
	/// </summary>
	/// <typeparam name="TMessage"></typeparam>
	public interface InstanceHandlerSelector<TMessage>
		where TMessage : class
	{
		IEnumerable<Action<IConsumeContext<TMessage>>> GetHandlers(IConsumeContext<TMessage> context);
	}
}