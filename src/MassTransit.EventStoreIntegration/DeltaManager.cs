namespace MassTransit.EventStoreIntegration
{
	using System;
	using System.Collections.Generic;
	using MassTransit.Util;

	/// <summary>
	/// Custom helper for event sourced sagas.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class DeltaManager<T>
		: ISagaDeltaManager
		where T : ISagaEventSourced
	{
		readonly Guid _sagaId;
		readonly IRouteEvents _eventRouter;
		ulong _version;

		private readonly ICollection<object> _uncommittedEvents = new LinkedList<object>();

		public DeltaManager([NotNull] ISagaEventSourced target, Guid sagaId, IRouteEvents eventRouter)
		{
			if (target == null) throw new ArgumentNullException("target");
			_sagaId = sagaId;
			_eventRouter = eventRouter ?? new ConventionEventRouter();
			_eventRouter.Register(target);
		}

		public ulong Version { get { return _version; } }

		public TDelta RaiseStateDelta<TDelta>([NotNull] TDelta delta)
			where TDelta : class
		{
			if (delta == null) throw new ArgumentNullException("delta");
			_uncommittedEvents.Add(delta);
			return delta;
		}

		void ISagaDeltaManager.ApplyStateDelta(object delta)
		{
			if (delta == null) throw new ArgumentNullException("delta");
			_eventRouter.Dispatch(delta);
			_version++;
		}

		IEnumerable<object> ISagaDeltaManager.GetUncommittedEvents()
		{
			return _uncommittedEvents;
		}

		void ISagaDeltaManager.ClearUncommittedEvents()
		{
			_uncommittedEvents.Clear();
		}

		public Guid CorrelationId { get { return _sagaId; } }
	}
}