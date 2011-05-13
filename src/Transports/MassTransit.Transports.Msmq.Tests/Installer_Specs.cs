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
namespace MassTransit.Transports.Msmq.Tests
{
	using Magnum.TestFramework;
	using Management;
	using NUnit.Framework;

	[TestFixture]
	public class When_verifying_the_msmq_installation
	{
		[Test, Explicit]
		public void Should_detect_the_presence_of_msmq()
		{
			var management = new MsmqManagement();

			management.IsInstalled().ShouldBeTrue();
		}


		[Test, Explicit]
		public void Should_install_for_the_version_of_windows()
		{
			var management = new MsmqManagement();

			management.Install();
		}
	}

	[TestFixture]
	public class When_verify_the_msdtc_installation
	{
		[Test, Explicit]
		public void Should_install_and_configure()
		{
			var msdtc = new MsDtcManagement();
			msdtc.VerifyConfiguration(true, true);
			msdtc.VerifyRunning(true);
		}
	}
}