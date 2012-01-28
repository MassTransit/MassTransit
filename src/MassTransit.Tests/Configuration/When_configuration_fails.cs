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
namespace MassTransit.Tests.Configuration
{
	using System;
	using System.Linq;
	using Configurators;
	using Exceptions;
	using Magnum.TestFramework;
	using NUnit.Framework;

	[TestFixture]
	public class When_configuration_fails
	{
		[Test]
		public void Should_include_the_configuration_results()
		{
			try
			{
				using (ServiceBusFactory.New(x => { }))
				{
				}

				Assert.Fail("An exception should have thrown for invalid configuration");
			}
			catch (ConfigurationException ex)
			{
				ex.Result.Results.Any(x => x.Disposition == ValidationResultDisposition.Failure)
					.ShouldBeTrue("There were no failure results");

				ex.Result.Results.Any(x => x.Key == "InputAddress")
					.ShouldBeTrue("There should have been an InputAddress violation");
			}
			catch (Exception ex)
			{
				Assert.Fail("The exception type thrown was invalid: " + ex.GetType().Name);
			}
		}

		[Test]
		public void Should_validate_against_null()
		{
			try
			{
				using (ServiceBusFactory.New(x =>
					{
						x.ReceiveFrom("loopback://localhost/a");
						x.UseBusBuilder(null);
					}))
				{
				}

				Assert.Fail("bus builder was set to null");
			}
			catch (ConfigurationException)
			{
			}
		}
	}
}