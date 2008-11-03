// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Host
{
	using System.Collections.Generic;
	using Actions;
	using ArgumentParsing;

    public class Parser
	{
        private static bool _isDefault;

        public static Args ParseArgs(string[] args)
		{
            if (args == null) args = new string[0];
            if(args.Length == 0)
                _isDefault = true;
            

            Args result = new Args();
			IArgumentMapFactory _argumentMapFactory = new ArgumentMapFactory();
			IArgumentParser _argumentParser = new ArgumentParser();
			IEnumerable<IArgument> arguments = _argumentParser.Parse(args);
			IArgumentMap mapper = _argumentMapFactory.CreateMap(result);
			IEnumerable<IArgument> remaining = mapper.ApplyTo(result, arguments);

			return result;
		}

		public class Args
		{
			private bool _console;
			private bool _gui;
			private bool _install;
			private bool _service;
			private bool _uninstall;


			[Argument(Key = "install")]
			public bool Install
			{
				get { return _install; }
				set { _install = value; }
			}

			[Argument(Key = "uninstall")]
			public bool Uninstall
			{
				get { return _uninstall; }
				set { _uninstall = value; }
			}

			[Argument(Key = "console")]
			public bool Console
			{
				get { return _console; }
				set { _console = value; }
			}

			[Argument(Key = "gui")]
			public bool Gui
			{
				get { return _gui; }
				set { _gui = value; }
			}

			[Argument(Key = "service")]
			public bool Service
			{
				get { return _service; }
				set { _service = value; }
			}

		    public bool IsDefault
		    {
		        get { return _isDefault; }
		    }


		    public NamedAction GetActionKey()
            {
                if (Install) return ServiceNamedAction.Install;
                if (Uninstall) return ServiceNamedAction.Uninstall;
                if (Service) return ServiceNamedAction.Service;
                if (Gui) return NamedAction.Gui;
                return NamedAction.Console;
            }
		}
	}
}