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
namespace MassTransit.RabbitMqTransport.Testing
{
    using MassTransit.Testing;
    using MassTransit.Testing.TestInstanceConfigurators;


    /// <summary>
	/// Extensions for configuring a test RabbitMQ instance that can be used
	/// in the test. See <see cref="RabbitMqBusTestScenarioBuilder"/> docs.
	/// </summary>
	public static class BusTestScenarioExtensions
	{
		/// <summary>
		/// Extensions for configuring a test RabbitMQ instance that can be used
		/// in the test. See <see cref="RabbitMqBusTestScenarioBuilder"/> docs.
		/// 
		/// Sample usage:
		/// <code>
		///using Magnum.TestFramework;
		///using MassTransit.Testing;
		///[Scenario]
		///public class Using_the_handler_test_factory
		///{
		///    IHandlerTest&lt;A&gt; _test;
		///
		///    [When]
		///    public void Setup()
		///    {
		///        _test = TestFactory.ForHandler&lt;A&gt;()
		///            .New(x =>
		///                {
		///                    x.UseRabbitMqBusScenario();
		///                    x.Send(new A());
		///                    x.Send(new B());
		///                });
		///        _test.Execute();
		///    }
		///    [Finally]
		///    public void Teardown()
		///    {
		///        _test.Dispose();
		///        _test = null;
		///    }
		///    [Then]
		///    public void Should_have_received_a_message_of_type_a()
		///    {
		///        _test.Received.Select&lt;A&gt;().ShouldBeTrue();
		///    }
		///}
		///</code>
		/// </summary>
		/// <param name="configurator">The configurator that is passed via the lambda that you are calling this method from.</param>
		public static void UseRabbitMqBusScenario(this ITestConfigurator<IBusTestScenario> configurator)
		{
//			configurator.UseScenarioBuilder(() => new RabbitMqBusTestScenarioBuilder());
		}
	}
}