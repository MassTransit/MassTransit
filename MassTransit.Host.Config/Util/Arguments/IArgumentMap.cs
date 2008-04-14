namespace MassTransit.Host.Config.Util.Arguments
{
	using System.Collections.Generic;

	public interface IArgumentMap
	{
		/// <summary>
		/// Applies the arguments to the specified object as they are enumerated
		/// </summary>
		/// <param name="objectToApplyTo">The object onto which the arguments should be applied</param>
		/// <param name="arguments">An enumerator of arguments being applied</param>
		IEnumerable<IArgument> ApplyTo(object objectToApplyTo, IEnumerable<IArgument> arguments);
		IEnumerable<IArgument> ApplyTo(object objectToApplyTo, IEnumerable<IArgument> arguments, ArgumentIntercepter intercepter);

		string Usage { get; }
	}

	public delegate bool ArgumentIntercepter(string name, string value);
}