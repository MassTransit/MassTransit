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
				x.Add((from arg in x.Argument("exit")
				       select (Command) new ExitCommand())
					.Or(from arg in x.Argument("quit")
					    select (Command) new ExitCommand())
//						.Or(from arg in x.Argument("count")
	//						select (Command)new StartOption())
//						.Or(from arg in x.Argument("exit")
//							select (Command)new HelpOption())
//						.Or(from arg in x.Argument("stop")
//							select (Option)new StopOption())
//						.Or(from arg in x.Switch("sudo")
//							select (Option)new SudoOption())
//						.Or(from arg in x.Argument("run")
//							select (Option)new RunOption())
						.Or(from arg in x.Argument("count")
							from uri in x.Definition("uri")
							select (Command)new CountCommand(uri.Value))
						.Or(from arg in x.Argument("move")
							from fromUri in x.Definition("from")
							from toUri in x.Definition("to")
							select (Command)new MoveCommand(fromUri.Value, toUri.Value, 1))
						.Or(from arg in x.Argument("move")
							from fromUri in x.Definition("from")
							from toUri in x.Definition("to")
							from count in x.Definition("count")
							select (Command)new MoveCommand(fromUri.Value, toUri.Value, int.Parse(count.Value)))
						.Or(from arg in x.Argument("peek")
							from uri in x.Definition("uri")
							select (Command)new PeekCommand(uri.Value, 1))
//						.Or(from autostart in x.Switch("autostart")
//							select (Option)new AutostartOption())
//						.Or(from interactive in x.Switch("interactive")
//							select (Option)new InteractiveOption())
//						.Or(from autostart in x.Switch("localservice")
//							select (Option)new LocalServiceOption())
//						.Or(from autostart in x.Switch("networkservice")
//							select (Option)new NetworkServiceOption())
//						.Or(from autostart in x.Switch("help")
//							select (Option)new HelpOption())
//						.Or(from svcname in x.Definition("servicename")
//							select (Option)new ServiceNameOption(svcname.Value))
//						.Or(from desc in x.Definition("description")
//							select (Option)new ServiceDescriptionOption(desc.Value))
//						.Or(from disp in x.Definition("displayname")
//							select (Option)new DisplayNameOption(disp.Value))
//						.Or(from instance in x.Definition("instance")
//							select (Option)new InstanceOption(instance.Value)));
					);
			}
	}
}