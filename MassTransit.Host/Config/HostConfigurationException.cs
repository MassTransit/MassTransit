using System;
using System.Collections.Generic;
using System.Text;

namespace MassTransit.Host.Config
{
	[Serializable]
	public class HostConfigurationException :
		Exception
	{
		public HostConfigurationException(string message)
			: base(message)
		{
		}
	}
}
