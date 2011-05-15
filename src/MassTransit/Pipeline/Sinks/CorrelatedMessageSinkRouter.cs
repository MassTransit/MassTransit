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
namespace MassTransit.Pipeline.Sinks
{
	using System.Collections.Generic;

	/// <summary>
	/// Routes a message to all of the connected message sinks without modification
	/// </summary>
	/// <typeparam name="TMessage">The type of the message to be routed</typeparam>
	/// <typeparam name="TKey">The correlation key type</typeparam>
	/// <typeparam name="T">The input type of the router</typeparam>
	public class CorrelatedMessageSinkRouter<T, TMessage, TKey> :
		MessageRouter<T>
		where TMessage : class, CorrelatedBy<TKey>
		where T : class
	{
		readonly TKey _correlationId;

		public CorrelatedMessageSinkRouter(TKey correlationId)
		{
			_correlationId = correlationId;
		}

		public CorrelatedMessageSinkRouter(TKey correlationId, IEnumerable<IPipelineSink<T>> sinks)
			: base(sinks)
		{
			_correlationId = correlationId;
		}

		public TKey CorrelationId
		{
			get { return _correlationId; }
		}
	}
}