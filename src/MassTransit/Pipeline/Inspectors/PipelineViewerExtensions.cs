namespace MassTransit
{
	using System;
	using Pipeline;
	using Pipeline.Inspectors;

	public static class PipelineViewerExtensions
	{
		public static void Trace<T>(this IPipelineSink<T> pipeline)
			where T : class
		{
			PipelineViewer.Trace(pipeline);
		}

		public static void View<T>(this IPipelineSink<T> pipeline, Action<string> callback)
			where T : class
		{
			PipelineViewer.Trace(pipeline, callback);
		}
	}
}