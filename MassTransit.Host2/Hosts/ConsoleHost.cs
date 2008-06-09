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
namespace MassTransit.Host2.Hosts
{
    using System;
    using log4net;

    public class ConsoleHost
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(ConsoleHost));
        private HostedEnvironment _environment;


        public ConsoleHost(HostedEnvironment environment)
        {
            _environment = environment;
        }

        public void Run(string[] args)
        {
            _log.Debug("Starting up as a console application");

            _environment.Start();

            Console.WriteLine("The service is running, press Control+C to exit.");
            Console.ReadKey();
            Console.WriteLine("Exiting.");

            _environment.Stop();
        }
    }
}