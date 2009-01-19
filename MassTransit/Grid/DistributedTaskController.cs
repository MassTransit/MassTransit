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
	using Messages;

	public class DistributedTaskController<TTask, TInput, TOutput> :
		Consumes<SubTaskWorkerAvailable<TInput>>.All,
		Consumes<SubTaskComplete<TOutput>>.All,
		Consumes<Fault<ExecuteSubTask<TInput>>>.All
		where TTask : class, IDistributedTask<TTask, TInput, TOutput>
		where TInput : class
		where TOutput : class
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (DistributedTaskController<TTask, TInput, TOutput>));

		private readonly IServiceBus _bus;
		private readonly TTask _distributedTask;
		private readonly IEndpointFactory _endpointFactory;
		private readonly Guid _taskId;
		private readonly Dictionary<string, Worker> _workers = new Dictionary<string, Worker>();
		private int _nextSubTask;
		private UnsubscribeAction _unsubscribeToken;

		public DistributedTaskController(IServiceBus bus, IEndpointFactory endpointFactory, TTask distributedTask)
		{
			_bus = bus;
			_endpointFactory = endpointFactory;
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
					_workers[message.Address].TaskLimit = message.TaskLimit;
				}

				// okay deliver the result
				_distributedTask.DeliverSubTaskOutput(message.SubTaskId, message.Output);
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
			_unsubscribeToken = _bus.Subscribe(this);

			_distributedTask.WhenCompleted(CompleteDistributedTask);

			_bus.Publish(new EnlistSubTaskWorkers<TInput>());
		}

		public class Worker
		{
			private readonly string _address;
			private int _activeTaskCount;
			private int _taskLimit;

			public Worker(string address)
			{
				_address = address;
			}

			public string Address
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

		private void CompleteDistributedTask(TTask obj)
		{
			_unsubscribeToken();
		}

		private void DispatchSubTaskToWorkers()
		{
			if (_nextSubTask >= _distributedTask.SubTaskCount)
				return;


			IList<Worker> workers = GetAvailableWorkers();
			while (workers.Count > 0)
			{
				foreach (Worker worker in workers)
				{
					if (_nextSubTask >= _distributedTask.SubTaskCount)
						return;

					IEndpoint endpoint = _endpointFactory.GetEndpoint(new Uri(worker.Address));
					if (endpoint == null)
						continue;

					ExecuteSubTask<TInput> executeExecuteSubTask;
					lock (_workers)
					{
						if (_nextSubTask >= _distributedTask.SubTaskCount)
							break;

						TInput input = _distributedTask.GetSubTaskInput(_nextSubTask);

						executeExecuteSubTask = new ExecuteSubTask<TInput>(_bus.Endpoint.Uri.ToString(), _taskId, _nextSubTask++, input);

						worker.ActiveTaskCount++;
					}

					endpoint.Send(executeExecuteSubTask);
				}

				workers = GetAvailableWorkers();
			}
		}

		private IList<Worker> GetAvailableWorkers()
		{
			List<Worker> workers = new List<Worker>();

			foreach (Worker worker in _workers.Values)
			{
				if (worker.ActiveTaskCount <= worker.TaskLimit)
				{
					workers.Add(worker);
				}
			}

			return workers;
		}
	}
}