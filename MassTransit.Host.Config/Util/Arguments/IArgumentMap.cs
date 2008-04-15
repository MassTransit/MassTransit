/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.
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