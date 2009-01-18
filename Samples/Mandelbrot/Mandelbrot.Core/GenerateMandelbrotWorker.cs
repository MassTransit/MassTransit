namespace Mandelbrot.Core
{
	using System;
	using MassTransit.Grid;

	public class GenerateMandelbrotWorker :
		ISubTaskWorker<GenerateRow, RowGenerated>
	{
		public const double OffsetX = -2.1;
		public const double OffsetY = -1.25;
		public const double SampleHeight = 2.5;
		public const double SampleWidth = 3.2;

		public void ExecuteTask(GenerateRow task, Action<RowGenerated> result)
		{
			RowGenerated rowGenerated = new RowGenerated
			                            	{
			                            		Data = new byte[task.Width]
			                            	};

			GenerateSingleRow(task.Width, task.Height, task.IterationLimit, rowGenerated.Data, task.Row);

			result(rowGenerated);
		}

		private static void GenerateSingleRow(int width, int height, int iterationLimit, byte[] data, int row)
		{
			for (int column = 0; column < width; column++)
			{
				double x = (column*SampleWidth)/width + OffsetX;
				double y = (row*SampleHeight)/height + OffsetY;

				data[column] = ComputeMandelbrotIndex(x, y, iterationLimit);
			}
		}

		protected static byte ComputeMandelbrotIndex(double x, double y, int iterationLimit)
		{
			double y0 = y;
			double x0 = x;

			for (int i = 0; i < iterationLimit; i++)
			{
				if (x*x + y*y >= 4)
				{
					return (byte) ((i%255) + 1);
				}
				double xtemp = x*x - y*y + x0;
				y = 2*x*y + y0;
				x = xtemp;
			}
			return 0;
		}
	}
}