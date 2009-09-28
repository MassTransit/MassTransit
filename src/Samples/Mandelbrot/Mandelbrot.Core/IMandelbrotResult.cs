namespace Mandelbrot.Core
{
	public interface IMandelbrotResult
	{
		int Height { get; }
		int Width { get; }
		int[] Data { get; }
	}
}