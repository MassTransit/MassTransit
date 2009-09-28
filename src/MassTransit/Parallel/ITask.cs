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
	using System.Collections.Generic;
	using Magnum.Monads;

	public interface ITask<T, R, TSubTask, TSubTaskResult>
		where T : class
		where TSubTask : ISplitTask<TSubTaskResult>
		where TSubTaskResult : class
	{
		/// <summary>
		/// Maps an input argument into a series of subtasks for processing
		/// </summary>
		/// <param name="argument"></param>
		/// <returns>An enumeration of subtasks to be processed</returns>
		K<IEnumerable<TSubTask>> Map(T argument);


		K<R> Reduce(IEnumerable<TSubTaskResult> results);
	}

	public interface ISplitTask<TResult>
		where TResult : class
	{
		void Cancel();

		TResult Execute();
	}

	public interface TaskFuture<R>
	{
		R Get();
		R Get(TimeSpan timeSpan);

		void Cancel();

		bool IsComplete();

		bool IsCancelled();
	}

	public interface TaskSession
	{
		Guid OriginatorNodeId { get; }
		Guid TaskId { get; }
		DateTime StartedAt { get; }
	}
}