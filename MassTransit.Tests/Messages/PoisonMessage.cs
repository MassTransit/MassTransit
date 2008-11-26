namespace MassTransit.Tests
{
	using System;

	[Serializable]
	public class PoisonMessage
	{
		public string ThrowException()
		{
			throw new Exception("POISON");
		}
	}
}