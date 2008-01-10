namespace MassTransit.ServiceBus
{
	/// <summary>
	/// This interface should be implemented by classes that want to automatically register
	/// message consumers with the service bus when loaded. The service bus bootloader will
	/// enumerate the classes in any references assemblies to determine if the interface is 
	/// supported and call those classes to register their handlers on the service bus.
	/// </summary>
	public interface IAutoSubscriber
	{
		/// <summary>
		/// Called by the service bus to allow the class to subscribe to handled
		/// message types
		/// </summary>
		/// <param name="bus">The instance of the service bus being initialized.</param>
		void Subscribe(IServiceBus bus);

		/// <summary>
		/// Called by the service bus to unsubscribe any handlers that were previously
		/// registered with the service bus.
		/// </summary>
		/// <param name="bus">The instance of the service bus being shut down.</param>
		void Unsubscribe(IServiceBus bus);
	}
}