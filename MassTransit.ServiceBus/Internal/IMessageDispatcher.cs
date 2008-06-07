/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.
namespace MassTransit.ServiceBus.Internal
{
	using System;

	/// <summary>
	/// A correlated message dispatcher sends a message to any attached consumers
	/// with a matching correlation identifier
	/// </summary>
	public interface IMessageDispatcher : IDisposable
	{
		void Consume(object message);
		bool Accept(object message);
	}

	public interface IMessageDispatcher<TMessage> : 
		IMessageDispatcher,
		Consumes<TMessage>.Selected,
		Produces<TMessage>
		where TMessage : class
	{
	}

	public interface Produces<TMessage> where TMessage : class
	{
		void Attach(Consumes<TMessage>.All consumer);
		void Detach(Consumes<TMessage>.All consumer);
	}
}