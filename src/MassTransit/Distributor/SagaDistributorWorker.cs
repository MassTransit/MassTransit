namespace MassTransit.Distributor
{
	using System;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Threading;
	using Internal;
	using Magnum.Actors;
	using Magnum.Actors.CommandQueues;
	using Magnum.Reflection;
	using Messages;
	using Saga;

	public class SagaDistributorWorker<T> :
		ISagaDistributorWorker<T>,
		Consumes<PrimeWorker>.All
		where T : SagaStateMachine<T>, ISaga
	{
		private readonly int _pending;
		private IServiceBus _bus;
		private IServiceBus _controlBus;
		private int _inProgress;
		private int _inProgressLimit = 4;
		private int _pendingLimit = 16;
		private UnsubscribeAction _unsubscribeAction = () => false;
		private Uri _dataUri;
		private Uri _controlUri;
		private CommandQueue _queue = new ThreadPoolCommandQueue();

		public SagaDistributorWorker(Func<T, Action<T>> getConsumer)
			: this(new DistributedConsumerSettings())
		{
		}

		public SagaDistributorWorker(DistributedConsumerSettings settings)
		{

			_inProgress = 0;
			_inProgressLimit = settings.InProgressLimit;
			_pending = 0;
			_pendingLimit = settings.PendingLimit;
		}

		public void Consume(Distributed<T> message)
		{
			Action<T> consumer = null;// _getConsumer(message.Payload);

			Interlocked.Increment(ref _inProgress);
			try
			{
				RewriteResponseAddress(message.ResponseAddress);

				consumer(message.Payload);
			}
			finally
			{
				Interlocked.Decrement(ref _inProgress);

				if (_inProgress == 0)
				{
					_queue.Enqueue(PublishWorkerAvailability);
					_bus.Endpoint.Send(new PrimeWorker());
				}

				var disposal = consumer as IDisposable;
				if (disposal != null)
				{
					disposal.Dispose();
				}
			}
		}

		public bool Accept(Distributed<T> message)
		{
			if (_inProgress >= _inProgressLimit)
				return false;

			return true;
		}

		public void Dispose()
		{
			_controlBus = null;
		}

		public void Start(IServiceBus bus)
		{
			_bus = bus;
			_controlBus = bus.ControlBus;

			_dataUri = _bus.Endpoint.Uri;
			_controlUri = _controlBus.Endpoint.Uri;

			_unsubscribeAction = bus.ControlBus.Subscribe<ConfigureDistributedConsumer<T>>(Consume);
			_unsubscribeAction += bus.Subscribe(this);

			PublishWorkerAvailability();
		}

//		private void frack<V>()
//		{
//			Type messageType = typeof (V);
//
//			var removeExpression = SagaStateMachine<T>.GetCompletedExpression();
//
//			ISagaPolicy<TSaga, V> policy = _policyFactory.GetPolicy<TSaga, V>(states, removeExpression);
//
//			Expression<Func<TSaga, V, bool>> expression;
//			if (SagaStateMachine<TSaga>.TryGetCorrelationExpressionForEvent(eevent, out expression))
//			{
//				return this.Call<UnsubscribeAction>("ConnectSink", eevent, policy, expression);
//			}
//
//			// we check for a standard correlation interface second
//			if (messageType.GetInterfaces().Where(x => x == typeof (CorrelatedBy<Guid>)).Count() > 0)
//			{
//				return this.Call<UnsubscribeAction>("ConnectCorrelatedSink", eevent, policy);
//			}
//
//			throw new NotSupportedException("No method to connect to event was found for " + typeof (V).FullName);
//		}

		public void Stop()
		{
			_unsubscribeAction();
		}

		private void Consume(ConfigureDistributedConsumer<T> message)
		{
			if (message.InProgressLimit >= 0)
				_inProgressLimit = message.InProgressLimit;

			if (message.PendingLimit >= 0)
				_pendingLimit = message.PendingLimit;

			PublishWorkerAvailability();
		}

		private void PublishWorkerAvailability()
		{
			_bus.Publish(new WorkerAvailable<T>(_controlUri, _dataUri, _inProgress, _inProgressLimit, _pending, _pendingLimit));
		}

		private static void RewriteResponseAddress(Uri responseAddress)
		{
			InboundMessageHeaders.SetCurrent(x => x.SetResponseAddress(responseAddress));
		}

		public void Consume(PrimeWorker message)
		{
		}
	}
}