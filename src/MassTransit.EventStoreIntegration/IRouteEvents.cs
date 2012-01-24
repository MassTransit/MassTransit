namespace MassTransit.EventStoreIntegration
{
	using System;

	public interface IRouteEvents
	{
		void Register<T>(Action<T> handler);
		void Register(ISagaEventSourced saga);
		void Dispatch(object eventMessage);
	}
}