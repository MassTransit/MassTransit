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
namespace MassTransit.Grid
{
	using System.Threading;
	using Messages;
	using ServiceBus;

	public class SubTaskWorker<TWorker, TInput, TOutput> :
		Consumes<ExecuteSubTask<TInput>>.All,
		Consumes<EnlistSubTaskWorkers<TInput>>.All
		where TInput : class
		where TOutput : class
		where TWorker : class, ISubTaskWorker<TInput, TOutput>
	{
		private static int _activeTaskCount;
		private static int _taskLimit = 2;

		public IServiceBus Bus { get; set; }
		public IObjectBuilder Builder { get; set; }

		public void Consume(EnlistSubTaskWorkers<TInput> message)
		{
			Bus.Publish(new SubTaskWorkerAvailable<TInput>(Bus.Endpoint.Uri.ToString(), _taskLimit, _activeTaskCount));
		}

		public void Consume(ExecuteSubTask<TInput> message)
		{
            TWorker worker = Builder.GetInstance<TWorker>();
			try
			{
				Interlocked.Increment(ref _activeTaskCount);

				worker.ExecuteTask(message.Task,
				                   output => Bus.Publish(new SubTaskComplete<TOutput>(Bus.Endpoint.Uri.ToString(),
				                                                                 _taskLimit,
				                                                                 _activeTaskCount,
				                                                                 message.TaskId,
				                                                                 message.SubTaskId, 
																				 output)));
			}
			finally
			{
				Builder.Release(worker);

				Interlocked.Decrement(ref _activeTaskCount);
			}
		}
	}
}