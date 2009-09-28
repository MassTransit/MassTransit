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
namespace MassTransit.Parallel
{
	using System;

	/// <summary>
	/// Required by a class that performs a distributed task
	/// </summary>
	/// <typeparam name="TTask">The type of the DistributedTask</typeparam>
	/// <typeparam name="TInput">The input class for the subtask</typeparam>
	/// <typeparam name="TOutput">The output class for the subtask</typeparam>
	public interface IDistributedTask<TTask, TInput, TOutput>
	{
		/// <summary>
		/// Returns the number of SubTasks in the DistributedTask
		/// </summary>
		int SubTaskCount { get; }

		/// <summary>
		/// Returns the input for the SubTask
		/// </summary>
		/// <param name="subTaskId">The identifier for the SubTask</param>
		/// <returns>The input class for the SubTask</returns>
		TInput GetSubTaskInput(int subTaskId);

		/// <summary>
		/// Delivers the output from a SubTask to the DistributedTask
		/// </summary>
		/// <param name="subTaskId">The identifier for the SubTask</param>
		/// <param name="output">The output data from processing the SubTask</param>
		void DeliverSubTaskOutput(int subTaskId, TOutput output);

		/// <summary>
		/// Notify the DistributedTask that an exception occurred in a worker
		/// </summary>
		/// <param name="subTaskId">The identifier for the SubTask</param>
		/// <param name="ex">The exception that was caught</param>
		void NotifySubTaskException(int subTaskId, Exception ex);

		/// <summary>
		/// Specifies a delegate to be called when the DistributedTask has completed
		/// </summary>
		/// <param name="action"></param>
		void WhenCompleted(Action<TTask> action);
	}
}