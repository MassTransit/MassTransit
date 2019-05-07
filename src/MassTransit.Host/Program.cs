// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System;
    using Serilog;
    using Topshelf;


    class Program
    {
        static ILogger _baseLogger;

        static int Main()
        {
            SetupLogger();

            return (int)HostFactory.Run(x =>
            {
                x.SetStartTimeout(TimeSpan.FromMinutes(2));

                var configurator = new MassTransitHostConfigurator<MassTransitHostServiceBootstrapper>
                {
                    Description = "MassTransit Host - A service host for MassTransit endpoints",
                    DisplayName = "MassTransit Host",
                    ServiceName = "MassTransitHost"
                };

                configurator.Configure(x);
            });
        }

        static void SetupLogger()
        {
            _baseLogger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("log\\MassTransit.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Logging.Logger.UseLoggerFactory(new SerilogLoggerFactory(_baseLogger));
        }
    }
}
