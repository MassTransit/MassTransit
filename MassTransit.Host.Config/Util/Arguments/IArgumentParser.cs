namespace MassTransit.Host.Config.Util.Arguments
{
	using System.Collections.Generic;

	public interface IArgumentParser
	{
		IList<IArgument> Parse(string[] args);
	}
}