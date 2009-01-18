namespace Mandelbrot.Core
{
	public class ImageGenerator :
		MandelbrotGeneratorBase
	{
		public ImageGenerator(int width, int iterationLimit)
			: base(width, iterationLimit)
		{
		}

		public override void Generate()
		{
			int index = 0;
			for (int row = 0; row < Height; row++)
			{
				for (int col = 0; col < Width; col++)
				{
					Data[index++] = ComputeMandelbrotIndex(row, col);
				}
			}
		}
	}
}