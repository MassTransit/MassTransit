// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Containers.Tests
{
    using Logging;
    using NUnit.Framework;
    using Serilog;
    using SerilogIntegration;


    [SetUpFixture]
    public class ContextSetup
    {
        Serilog.ILogger _baseLogger;

        [OneTimeSetUp]
        public void Before_any_tests()
        {
            _baseLogger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            SerilogLogger.Use(_baseLogger);
        }

        [OneTimeTearDown]
        public void After_all_tests()
        {
            Logger.Shutdown();
        }
    }
}