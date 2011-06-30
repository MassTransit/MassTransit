// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace BusDriver
{
	using System.Collections.Generic;
	using System.Linq;
	using Commands;
	using Magnum.CommandLineParser;
	using Magnum.Monads.Parser;

	public class CommandParser
	{
		public static bool Parse(string commandText)
		{
			return CommandLine.Parse<Command>(commandText, InitializeCommandLineParser)
				.All(option => option.Execute());
		}

		static void InitializeCommandLineParser(ICommandLineElementParser<Command> x)
		{
			Parser<IEnumerable<ICommandLineElement>, IDefinitionElement> definitions =
				(from output in x.Definition("out") select output);

			x.Add((from arg in x.Argument("exit")
			       select (Command) new ExitCommand())
				.Or(from arg in x.Argument("quit")
				    select (Command) new ExitCommand())
				.Or(from arg in x.Argument("help")
				    select (Command) new HelpCommand())
				.Or(from arg in x.Argument("set")
				    from arg2 in x.Argument("uri")
				    from uri in x.Argument()
				    select (Command) new SetUriCommand(uri.Id))
				.Or(from arg in x.Argument("count")
				    from uri in (from d in x.Definition("uri") select d).Optional("uri", Program.CurrentUri)
				    select (Command) new CountCommand(uri.Value))
				.Or(from arg in x.Argument("move")
				    from fromUri in x.Definition("from")
				    from toUri in x.Definition("to")
				    from count in
				    	(from d in x.Definition("count") select d).Optional("count", "100")
				    select (Command) new MoveCommand(fromUri.Value, toUri.Value, int.Parse(count.Value)))
				.Or(from arg in x.Argument("peek")
				    from uri in
				    	(from d in x.Definition("uri") select d).Optional("uri", Program.CurrentUri)
				    select (Command) new PeekCommand(uri.Value, 1))
				.Or(from arg in x.Argument("trace")
				    from uri in
				    	(from d in x.Definition("uri") select d).Optional("uri", Program.CurrentUri)
				    from count in
				    	(from d in x.Definition("count") select d).Optional("count", "100")
				    select (Command) new TraceCommand(uri.Value, int.Parse(count.Value)))
				);
		}
	}
}