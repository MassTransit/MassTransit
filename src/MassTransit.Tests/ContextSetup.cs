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
namespace MassTransit.Tests
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using Log4NetIntegration.Logging;
    using Logging;
    using NUnit.Framework;
    using log4net;
    using log4net.Config;

    [SetUpFixture]
    public class ContextSetup
    {
        [OneTimeSetUp]
        public void Before_any()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string file = Path.Combine(path, "masstransit.tests.log4net.xml");

#if NETCORE
            var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo(file));
#else
            XmlConfigurator.Configure(new FileInfo(file));
#endif
            Trace.WriteLine("Loading Log4net: " + file);

            Logger.UseLogger(new Log4NetLogger());
        }

        [OneTimeTearDown]
        public void After_all()
        {
            LogManager.Shutdown();
        }
    }
}