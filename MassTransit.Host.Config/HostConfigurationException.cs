using System;

namespace MassTransit.Host.Config
{
	public class HostConfigurationException : Exception
	{
		public HostConfigurationException(string message)
			: base(message)
		{
		}
	}
}