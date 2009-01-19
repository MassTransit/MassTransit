namespace Mandelbrot.Core
{
	using System;
	using MassTransit.Grid;

	public class GenerateMandelbrotWorker :
		ISubTaskWorker<GenerateRow, RowGenerated>
	{
		public void ExecuteTask(GenerateRow task, Action<RowGenerated> result)
		{
			int[] data = new int[task.Width];
			for (int column = 0; column < task.Width; column++)
			{
				double x = (column*task.SampleWidth)/task.Width + task.OffsetX;
				double y = (task.Row*task.SampleHeight)/task.Height + task.OffsetY;

				data[column] = ComputeMandelbrotIndex(x, y, task.IterationLimit);
			}

			var rowGenerated = new RowGenerated {Data = data};

			result(rowGenerated);
		}

		protected static int ComputeMandelbrotIndex(double x, double y, int iterationLimit)
		{
			double y0 = y;
			double x0 = x;

			for (int i = 0; i < iterationLimit; i++)
			{
				if (x*x + y*y >= 4)
				{
					return i + 1;
				}
				double xtemp = x*x - y*y + x0;
				y = 2*x*y + y0;
				x = xtemp;
			}
			return 0;
		}
	}
}