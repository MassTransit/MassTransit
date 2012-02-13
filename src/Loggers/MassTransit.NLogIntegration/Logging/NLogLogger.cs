using MassTransit.Logging;
using MassTransit.NLogIntegration.Logging;

namespace MassTransit.NLogIntegration
{
	public class NLogLogger : ILogger
	{
		public ILog Get(string name)
		{
			return new NLogLog(name);
		}
	}
}