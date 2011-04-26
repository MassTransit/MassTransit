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
				var bus = ServiceBusFactory.New(x =>
					{
					});

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
	}
}
