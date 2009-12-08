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
namespace MassTransit.Distributor
{
	using System;
	using System.Linq;
    using System.Threading;
	using Magnum;
	using Magnum.Threading;
    using Magnum.Actors.Schedulers;
    using Magnum.DateTimeExtensions;
	using Messages;

	public class Distributor<T> :
		IDistributor<T>,
		Consumes<T>.Selected
		where T : class
	{
		private readonly IEndpointFactory _endpointFactory;
		private readonly ReaderWriterLockedDictionary<Uri, WorkerDetails> _workers = new ReaderWriterLockedDictionary<Uri, WorkerDetails>();
		private IWorkerSelectionStrategy<T> _selectionStrategy;
		private UnsubscribeAction _unsubscribeAction = () => false;
	    private ThreadPoolScheduler _threadPoolScheduler;
	    private Timer _pinger;

        public Distributor(IEndpointFactory endpointFactory, IWorkerSelectionStrategy<T> workerSelectionStrategy)
        {
            _endpointFactory = endpointFactory;
			_selectionStrategy = workerSelectionStrategy;

            _threadPoolScheduler = new ThreadPoolScheduler();
        }

	    public Distributor(IEndpointFactory endpointFactory) :
            this(endpointFactory, new DefaultWorkerSelectionStrategy<T>())
		{
		}

		public void Consume(T message)
		{
			WorkerDetails worker = _selectionStrategy.GetAvailableWorkers(_workers.Values, message).FirstOrDefault();
			if (worker == null)
			{
				CurrentMessage.RetryLater();
				return;
			}

			worker.Add();

			IEndpoint endpoint = _endpointFactory.GetEndpoint(worker.DataUri);

			var distributed = new Distributed<T>(message, CurrentMessage.Headers.ResponseAddress);

			endpoint.Send(distributed);
		}

		public bool Accept(T message)
		{
			return _selectionStrategy.GetAvailableWorkers(_workers.Values, message).Count() > 0;
		}

		public void Dispose()
		{
		}

		public void Start(IServiceBus bus)
		{
			_unsubscribeAction = bus.Subscribe<WorkerAvailable<T>>(Consume);

			// don't plan to unsubscribe this since it's an important thing
			bus.Subscribe(this);

		    int oneMinute = (int) 1.Minutes().TotalMilliseconds;
            _threadPoolScheduler.Schedule(oneMinute, oneMinute, PingWorkers);
		}

		public void Stop()
		{
            _threadPoolScheduler.Dispose();

			_workers.Clear();

			_unsubscribeAction();
		}

		public void Consume(WorkerAvailable<T> message)
		{
			WorkerDetails worker = _workers.Retrieve(message.ControlUri, () =>
				{
					return new WorkerDetails
						{
							ControlUri = message.ControlUri,
							DataUri = message.DataUri,
							InProgress = message.InProgress,
							InProgressLimit = message.InProgressLimit,
							LastUpdate = SystemUtil.UtcNow,
						};
				});

			worker.UpdateInProgress(message.InProgress, message.InProgressLimit, message.Updated);
		}

        private void PingWorkers()
        {
            _workers.Values
                .Where(x => x.LastUpdate < SystemUtil.UtcNow.Subtract(1.Minutes()))
                .ToList()
                .ForEach(x =>
            {
                _endpointFactory.GetEndpoint(x.ControlUri).Send(new PingWorker());
            });
        }
	}
}