namespace MassTransit.Patterns.FaultDetection.Messages
{
	using System;

	[Serializable]
	public class Heartbeat
	{
		public int Pulse;
	}
}