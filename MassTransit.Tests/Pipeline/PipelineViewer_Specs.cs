namespace MassTransit.Tests.Pipeline
{
	using System.Collections.Generic;
	using MassTransit.Pipeline;
	using NUnit.Framework;

	[TestFixture]
	public class When_working_with_an_existing_pipeline
	{
		[Test]
		public void I_want_to_display_the_entire_flow_through_the_pipeline()
		{
			MessagePipeline pipeline = MessagePipeline.CreateDefaultPipeline();

			PipelineViewer.Trace(pipeline);
		}
	}
}