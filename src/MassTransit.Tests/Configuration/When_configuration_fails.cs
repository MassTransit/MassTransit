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
	using NUnit.Framework;
	using Shouldly;

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
                    .ShouldBe(true); //"There were no failure results"

				ex.Result.Results.Any(x => x.Key == "InputAddress")
                    .ShouldBe(true); //"There should have been an InputAddress violation"
			}
			catch (Exception ex)
			{
				Assert.Fail("The exception type thrown was invalid: " + ex.GetType().Name);
			}
		}

		[Test, Description("It should be possible to introspect the configuration " +
		                   "as we're starting up to make sure that the programmer " +
						   "is warned to avoid strange exceptions and errors from serialization.")]
		public void Should_know_consumers_messages_without_default_ctors()
		{
			using (ServiceBusFactory.New(x =>
				{
//					x.Subscribe(s => s.Consumer<ConsumerOf<MessageWithNonDefaultCtor>>());
					x.ReceiveFrom("loopback://localhost/mt_queue");
				    x.Validate()
				     .Any(result => result.Disposition == ValidationResultDisposition.Warning
				         && result.Message.Contains("default")
				         && result.Key.Contains("Consumer"))
				     .ShouldBe(true);//True(string.Format("there should be a warning on message without default c'tors"));
				}))
			{
			}
		}

		[Serializable]
		public class MessageWithNonDefaultCtor
		{
			public MessageWithNonDefaultCtor(int someInt)
			{
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