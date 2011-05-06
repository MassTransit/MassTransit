namespace MassTransit
{
	using System;
	using Pipeline;
	using Pipeline.Inspectors;

	public static class PipelineViewerExtensions
	{
		public static void Trace(this IMessagePipeline pipeline)
		{
			PipelineViewer.Trace(pipeline);
		}

		public static void View(this IMessagePipeline pipeline, Action<string> callback)
		{
			PipelineViewer.Trace(pipeline, callback);
		}
	}
}