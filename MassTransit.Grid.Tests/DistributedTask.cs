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
namespace MassTransit.Grid.Tests
{
	using System;
	using System.Collections.Generic;
	using MassTransit.ServiceBus.Internal;
	using ServiceBus;

	public class DistributedTask<TTask, TSubTask, TResult> :
		Consumes<SubTaskWorkerAvailable<TSubTask>>.All,
		Consumes<SubTaskComplete<TResult>>.All
		where TTask : class, IDistributedTask<TSubTask, TResult>
		where TSubTask : class, ISubTask
		where TResult : class
	{
		private readonly IObjectBuilder _builder;
		private readonly IServiceBus _bus;
		private readonly TTask _distributedTask;
		private readonly Dictionary<Uri, Worker> _workers = new Dictionary<Uri, Worker>();
		private IEnumerator<TSubTask> _subTaskEnumerator;
		private Guid _taskId;
		private IEndpointResolver _endpointResolver;

		public DistributedTask(IServiceBus bus, IEndpointResolver endpointResolver, TTask distributedTask)
		{
			_bus = bus;
			_endpointResolver = endpointResolver;
			_distributedTask = distributedTask;

			_taskId = Guid.NewGuid();
		}

		public void Consume(SubTaskComplete<TResult> message)
		{
			try
			{
				// a response from another task by chance?
				if (message.TaskId != _taskId)
					return;

				// a response from a worker we don't know? not happening!
				if (_workers.ContainsKey(message.Address) == false)
					return;

				lock (_workers)
				{
					_workers[message.Address].ActiveTaskCount--;
				}

				// okay deliver the result
				_distributedTask.DeliverResult(message.SubTaskId, message.Result);
			}
			finally
			{
				DispatchSubTaskToWorkers();
			}
		}

		public void Consume(SubTaskWorkerAvailable<TSubTask> message)
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

			_bus.Publish(new EnlistSubTaskWorkers<TSubTask>());

			IEnumerable<TSubTask> enumerable = _distributedTask;
			if (enumerable == null)
				throw new NullReferenceException("No valid enumerator for this distributor");

			_subTaskEnumerator = enumerable.GetEnumerator();
		}

		private void DispatchSubTaskToWorkers()
		{
			IList<Worker> workers = GetAvailableWorkers();

			foreach (Worker worker in workers)
			{
				IEndpoint endpoint = _endpointResolver.Resolve(worker.Address);
				if (endpoint == null)
					continue;

				if (!_subTaskEnumerator.MoveNext())
					return;

				TSubTask subTask = _subTaskEnumerator.Current;

				ExecuteSubTask<TSubTask> executeExecuteSubTask = new ExecuteSubTask<TSubTask>(_bus.Endpoint.Uri, _taskId, subTask.TaskId, subTask);

				endpoint.Send(executeExecuteSubTask);

				worker.ActiveTaskCount++;
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