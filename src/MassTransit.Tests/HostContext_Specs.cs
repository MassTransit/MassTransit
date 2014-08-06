// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Context;
    using NUnit.Framework;


    [TestFixture]
    public class HostContext_Specs
    {
        HostContext _hostContext;

        [TestFixtureSetUp]
        public void Setup()
        {
            _hostContext = new MassTransitHostContext();
        }

        [Test]
        public void Should_contain_the_framework_version()
        {
            Assert.AreEqual(Environment.Version.ToString(), _hostContext.FrameworkVersion);
        }

        [Test]
        public void Should_contain_the_machine_name()
        {
            Assert.AreEqual(Environment.MachineName, _hostContext.HostName);
        }

        [Test]
        public void Should_contain_the_operating_system_version()
        {
            Assert.AreEqual(Environment.OSVersion.VersionString, _hostContext.OperatingSystemVersion);
        }

        [Test]
        public void Should_contain_the_assembly_version()
        {
            Assert.AreEqual(typeof(IServiceBus).Assembly.GetName().Version.ToString(), _hostContext.MassTransitVersion);
        }

        [Test]
        public void Should_contain_the_process_name_and_id()
        {
            var process = Process.GetCurrentProcess();

            Assert.AreEqual(process.Id, _hostContext.ProcessId);
            Assert.AreEqual(process.ProcessName, _hostContext.ProcessName);
        }

        [Test]
        public void Should_contain_entry_assembly_name()
        {
            Assert.IsNotNullOrEmpty(_hostContext.Assembly);
            Assert.IsNotNullOrEmpty(_hostContext.AssemblyVersion);

            Console.WriteLine("{0}: {1}", _hostContext.Assembly, _hostContext.AssemblyVersion);
        }
    }
}