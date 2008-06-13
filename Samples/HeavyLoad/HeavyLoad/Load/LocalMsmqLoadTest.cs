namespace HeavyLoad.Load
{
	public class LocalMsmqLoadTest : LocalLoadTest
	{
		private static readonly string _queueUri = "msmq://localhost/test_servicebus";

		public LocalMsmqLoadTest()
			: base()
		{
		}
	}
}