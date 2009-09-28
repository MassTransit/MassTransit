namespace Mandelbrot.Core
{
	using System;

	[Serializable]
	public class GenerateRow
	{
		public int Width { get; set; }

		public int Height { get; set; }

		public int Row { get; set; }

		public int IterationLimit { get; set; }

		public double OffsetX { get; set; }

		public double OffsetY { get; set; }

		public double SampleWidth { get; set; }

		public double SampleHeight { get; set; }
	}
}