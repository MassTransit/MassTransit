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
namespace MassTransit.Host2.ArgumentParsing
{
	using System.Collections.Generic;
	using MassTransit.Host2.ArgumentParsing;

    public class ArgumentParser : IArgumentParser
	{
		private static readonly char[] _switchChars = new char[] {':', '+', '-'};

		#region IArgumentParser Members

		public IList<IArgument> Parse(string[] args)
		{
			List<IArgument> arguments = new List<IArgument>();

			foreach (string arg in args)
			{
				if (arg.Length == 0)
					continue;

				switch (arg[0])
				{
					case '-':
					case '/':

						int end = arg.IndexOfAny(_switchChars, 1);
						if (end == -1)
						{
							Argument switchArg = new Argument(arg.Substring(1), "true");
							arguments.Add(switchArg);
						}
						else
						{
							string key = arg.Substring(1, end - 1);
							string value = "true";

							if (arg[end] == '-')
								value = "false";
							else if (arg[end] == ':')
							{
								if (arg.Length > key.Length + 2)
									value = arg.Substring(key.Length + 2);
								else
									value = string.Empty;
							}

							Argument switchArg = new Argument(key, value);
							arguments.Add(switchArg);
						}
						break;

					default:
						Argument argument = new Argument(null, arg);
						arguments.Add(argument);
						break;
				}
			}

			return arguments;
		}

		#endregion
	}
}