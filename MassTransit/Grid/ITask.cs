namespace MassTransit.Grid
{
	using System;
	using System.Collections.Generic;
	using Magnum.Common.Monads;

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