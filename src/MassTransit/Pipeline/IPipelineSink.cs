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

	public interface IPipelineSink
	{
	}

	/// <summary>
	/// Implemented by all classes that can be inserted into the pipeline
	/// </summary>
	/// <typeparam name="T">The message type passed by this sink</typeparam>
	public interface IPipelineSink<T> :
		IPipelineSink
		where T : class
	{
		/// <summary>
		/// Passes a message through the pipeline returning all consumers for the message
		/// so that it can be dispatched to those consumers. The message does not actually dispatch
		/// in the pipeline, the consumers Consume method is called.
		/// </summary>
		/// <param name="context"></param>
		/// <returns>An enumerable of consumers for the message</returns>
		IEnumerable<Action<T>> Enumerate(T context);

		/// <summary>
		/// Walks the pipeline from the current sink forward, calling back to the inspector for each
		/// sink in the pipeline.
		/// DING: Visitor Pattern merit badge awarded
		/// </summary>
		/// <param name="inspector">The inspector to call back to for each sink</param>
		/// <returns>True if the inspection should continue, false to stop</returns>
		bool Inspect(IPipelineInspector inspector);
	}
}