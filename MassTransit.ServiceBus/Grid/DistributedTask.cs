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
	using System;
	using System.Collections.Generic;
	using log4net;
	using MassTransit.ServiceBus.Internal;
	using Messages;
	using ServiceBus;

	public class DistributedTask<TTask, TInput, TOutput> :
		Consumes<SubTaskWorkerAvailable<TInput>>.All,
		Consumes<SubTaskComplete<TOutput>>.All,
		Consumes<Fault<ExecuteSubTask<TInput>>>.All
		where TTask : class, IDistributedTask<TTask, TInput, TOutput>
		where TInput : class
		where TOutput : class
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(DistributedTask<TTask,TInput,TOutput>));

		private readonly IServiceBus _bus;
		private readonly TTask _distributedTask;
		private readonly IEndpointResolver _endpointResolver;
		private readonly Guid _taskId;
		private readonly Dictionary<Uri, Worker> _workers = new Dictionary<Uri, Worker>();
		private int _nextSubTask = 0;

		public DistributedTask(IServiceBus bus, IEndpointResolver endpointResolver, TTask distributedTask)
		{
			_bus = bus;
			_endpointResolver = endpointResolver;
			_distributedTask = distributedTask;

			_taskId = Guid.NewGuid();
		}

		public void Consume(Fault<ExecuteSubTask<TInput>> message)
		{
			try
			{
				// a response from another task by chance?
				if (message.FailedMessage.TaskId != _taskId)
				{
					_log.DebugFormat("Fault received for unknown task: {0}", message.FailedMessage.TaskId);
					return;
				}

				// a response from a worker we don't know? not happening!
				if (_workers.ContainsKey(message.FailedMessage.Address) == false)
				{
					_log.DebugFormat("Fault received from unknown worker: {0}/{1}", message.FailedMessage.TaskId, message.FailedMessage.Address);
					return;
				}

				lock (_workers)
				{
					_workers[message.FailedMessage.Address].ActiveTaskCount--;
				}

				_distributedTask.NotifySubTaskException(message.FailedMessage.SubTaskId, message.CaughtException);

			}
			finally
			{
				DispatchSubTaskToWorkers();
			}
		}

		public void Consume(SubTaskComplete<TOutput> message)
		{
			try
			{
				// a response from another task by chance?
				if (message.TaskId != _taskId)
				{
					_log.DebugFormat("Output received for unknown task: {0}", message.TaskId);
					return;
				}

				// a response from a worker we don't know? not happening!
				if (_workers.ContainsKey(message.Address) == false)
				{
					_log.DebugFormat("Output received from unknown worker: {0}/{1}", message.TaskId, message.Address);
					return;
				}

				lock (_workers)
				{
					_workers[message.Address].ActiveTaskCount--;
				}

				// okay deliver the result
				_distributedTask.DeliverSubTaskOutput(message.SubTaskId, message.Result);
			}
			finally
			{
				DispatchSubTaskToWorkers();
			}
		}

		public void Consume(SubTaskWorkerAvailable<TInput> message)
		{
			try
			{
				lock (_workers)
				{
					if (!_workers.ContainsKey(message.Address))
						_workers.Add(message.Address, new Worker(message.Address));

					_workers[message.Address].TaskLimit = message.TaskLimit;
					_workers[message.Address].ActiveTaskCount = message.ActiveTaskCount;
				}
			}
			finally
			{
				DispatchSubTaskToWorkers();
			}
		}

		public void Start()
		{
			_bus.Subscribe(this);

			_distributedTask.WhenCompleted(CompleteDistributedTask);

			_bus.Publish(new EnlistSubTaskWorkers<TInput>());
		}

		private void CompleteDistributedTask(TTask obj)
		{
			_bus.Unsubscribe(this);

		}

		private void DispatchSubTaskToWorkers()
		{
			if (_nextSubTask >= _distributedTask.SubTaskCount)
				return;

			IList<Worker> workers = GetAvailableWorkers();

			foreach (Worker worker in workers)
			{
				IEndpoint endpoint = _endpointResolver.Resolve(worker.Address);
				if (endpoint == null)
					continue;

				ExecuteSubTask<TInput> executeExecuteSubTask;
				lock (_workers)
				{
					if (_nextSubTask >= _distributedTask.SubTaskCount)
						break;

					TInput input = _distributedTask.GetSubTaskInput(_nextSubTask);

					executeExecuteSubTask = new ExecuteSubTask<TInput>(_bus.Endpoint.Uri, _taskId, _nextSubTask++, input);

					worker.ActiveTaskCount++;
				}

				endpoint.Send(executeExecuteSubTask);
			}
		}

		private IList<Worker> GetAvailableWorkers()
		{
			List<Worker> workers = new List<Worker>();

			foreach (Worker worker in _workers.Values)
			{
				if (worker.ActiveTaskCount < worker.TaskLimit)
				{
					workers.Add(worker);
				}
			}

			return workers;
		}


		public class Worker
		{
			private readonly Uri _address;
			private int _activeTaskCount;
			private int _taskLimit;

			public Worker(Uri address)
			{
				_address = address;
			}

			public Uri Address
			{
				get { return _address; }
			}

			public int TaskLimit
			{
				get { return _taskLimit; }
				set { _taskLimit = value; }
			}

			public int ActiveTaskCount
			{
				get { return _activeTaskCount; }
				set { _activeTaskCount = value; }
			}
		}
	}
}