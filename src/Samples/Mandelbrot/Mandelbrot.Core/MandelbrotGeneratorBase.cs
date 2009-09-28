namespace Mandelbrot.Core
{
	public abstract class MandelbrotGeneratorBase
	{
		public const double SampleWidth = 3.2;
		public const double SampleHeight = 2.5;
		public const double OffsetX = -2.1;
		public const double OffsetY = -1.25;

		public int Height { get; private set; }
		public int Width { get; private set; }
		public int MaxIterations { get; private set; }
		public byte[] Data { get; private set; }

		protected MandelbrotGeneratorBase(int width, int maxIterations)
		{
			Width = width;
			Height = GetHeight(width);
			MaxIterations = maxIterations;
			Data = new byte[Height * Width];
		}

		static int GetHeight(int width)
		{
			return (int)((SampleHeight * width) / SampleWidth);
		}

		public abstract void Generate();

		/// <summary>
		/// Method used by many tests to compute the correct byte value for a single point
		/// </summary>
		protected byte ComputeMandelbrotIndex(int row, int col)
		{
			double x = (col * SampleWidth) / Width + OffsetX;
			double y = (row * SampleHeight) / Height + OffsetY;

			double y0 = y;
			double x0 = x;

			for (int i = 0; i < MaxIterations; i++)
			{
				if (x * x + y * y >= 4)
				{
					return (byte)((i % 255) + 1);
				}
				double xtemp = x * x - y * y + x0;
				y = 2 * x * y + y0;
				x = xtemp;
			}
			return 0;
		}
	}
}