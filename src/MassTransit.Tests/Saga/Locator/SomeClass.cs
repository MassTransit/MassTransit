namespace MassTransit.Tests.Saga.Locator
{
	public class SomeClass
	{
		private IServiceBus _bus;

		public SomeClass(IServiceBus bus)
		{
			_bus = bus;
		}

		public void DoSomething()
		{
		}
	}
}