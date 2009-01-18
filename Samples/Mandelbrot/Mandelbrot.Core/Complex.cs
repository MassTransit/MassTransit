namespace Mandelbrot.Core
{
	public struct Complex
	{
		private readonly double _imaginary;
		private readonly double _real;

		public Complex(double real, double imaginary)
		{
			_real = real;
			_imaginary = imaginary;
		}

		public double SquareLength
		{
			get { return _real*_real + _imaginary*_imaginary; }
		}

		public static Complex operator +(Complex c1, Complex c2)
		{
			return new Complex(c1._real + c2._real, c1._imaginary + c2._imaginary);
		}

		public static Complex operator *(Complex c1, Complex c2)
		{
			return new Complex(c1._real*c2._real - c1._imaginary*c2._imaginary,
			                   c1._real*c2._imaginary + c2._real*c1._imaginary);
		}
	}
}