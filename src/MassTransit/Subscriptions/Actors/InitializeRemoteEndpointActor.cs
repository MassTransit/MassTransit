namespace MassTransit.Subscriptions.Actors
{
	using System;

	public class InitializeRemoteEndpointActor
	{
		public Uri ControlUri { get; set; }
	}
}