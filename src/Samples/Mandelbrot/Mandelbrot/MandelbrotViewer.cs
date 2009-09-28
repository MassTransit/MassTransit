namespace Mandelbrot
{
	using System;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Windows.Forms;
	using Core;

	public partial class MandelbrotViewer :
		Control
	{
		private Bitmap _offScreenBuffer;
		private Graphics _offScreenGraphics;
		private Color[] _palette;

		public MandelbrotViewer()
		{
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.UserPaint, true);

			CreateDefaultPalette();
		}

		private void CreateDefaultPalette()
		{
			_palette = new Color[1024];

			for (int i = 0; i < 64; i++)
			{
				_palette[i + 1] = Color.FromArgb(255, 255, (byte)(255 - (i * 4)));
			}
			//create yellow to orange in positions 64-127
			for (int i = 0; i < 64; i++)
			{
				_palette[i + 64 + 1] = Color.FromArgb(255, (byte)(255 - (i * 2)), 0);
			}
			//create orange to red in positions 128-191
			for (int i = 0; i < 64; i++)
			{
				_palette[i + 128 + 1] = Color.FromArgb(255, (byte)(128 - (i * 2)), 0);
			}
			//create red to purple in positions 192-255
			for (int i = 0; i < 64; i++)
			{
				_palette[i + 192 + 1] = Color.FromArgb(
					(byte)(i == 63 ? 0 : 255 - i * 4),
					(byte)(255 - (i * 2)), 0, (byte)(i * 2));
			}
			//create purple to white
			for (int i = 0; i < 64; i++)
			{
				_palette[i + 256 + 1] = Color.FromArgb(
					(byte)(i == 63 ? 255 : i * 4),
					(byte)((i * 2)), 0, (byte)(i * 2));
			}
		}

		protected override void OnPaint(PaintEventArgs paintEventArgs)
		{
			paintEventArgs.Graphics.DrawImage(_offScreenBuffer, 0, 0);
		}

		protected override void OnResize(EventArgs e)
		{
			// if size changed, recreate buffer bitmap
			if ((_offScreenBuffer == null) || (_offScreenBuffer.Size != Size))
			{
				if ((Size.Width != 0) && (Size.Height != 0))
				{
					_offScreenBuffer = new Bitmap(Size.Width, Size.Height);
					_offScreenGraphics = Graphics.FromImage(_offScreenBuffer);

					_offScreenGraphics.SmoothingMode = SmoothingMode.AntiAlias;
				}
			}

			base.OnResize(e);
		}

		public void UpdateDisplayWithResults(IMandelbrotResult result)
		{
			int width = Math.Min(Width, result.Width);
			int height = Math.Min(Height, result.Height);

			int[] data = result.Data;

			//CreateSmoothPalette(Color.FromKnownColor(KnownColor.Blue), Color.FromKnownColor(KnownColor.Yellow), max+1);

			SolidBrush black = new SolidBrush(Color.FromKnownColor(KnownColor.Black));

			_offScreenGraphics.FillRectangle(black, 0, 0, Width, Height);

			for (int row = 0; row < height; row++)
			{
				int offset = row*result.Width;
				for (int column = 0; column < width; column++)
				{
					_offScreenBuffer.SetPixel(column, row, _palette[data[offset++]]);
				}
			}

			Invalidate();
		}

		public void CreateSmoothPalette(Color start, Color end, int range)
		{
			_palette = new Color[range + 1];

			double startH = start.GetHue();
			double startS = start.GetSaturation();
			double startV = start.GetBrightness();

			double stopH = end.GetHue();
			double stopS = end.GetSaturation();
			double stopV = end.GetBrightness();

			for (int i = 0; i < range; i++)
			{
				double h = (stopH - startH)*i + startH;
				double s = (stopS - startS)*i + startS;
				double v = (stopV - startV)*i + startV;

				_palette[i] = GetColorFromHSV(h, s, v);
			}
		}

		private Color GetColorFromHSV(double h, double s, double v)
		{
			int r, g, b;

			HsvToRgb(h, s, v, out r, out g, out b);

			return Color.FromArgb(r, g, b);
		}

		private void HsvToRgb(double h, double S, double V, out int r, out int g, out int b)
		{
			// ######################################################################
			// T. Nathan Mundhenk
			// mundhenk@usc.edu
			// C/C++ Macro HSV to RGB

			double H = h;
			while (H < 0)
			{
				H += 360;
			}
			;
			while (H >= 360)
			{
				H -= 360;
			}
			;
			double R, G, B;
			if (V <= 0)
			{
				R = G = B = 0;
			}
			else if (S <= 0)
			{
				R = G = B = V;
			}
			else
			{
				double hf = H/60.0;
				int i = (int) Math.Floor(hf);
				double f = hf - i;
				double pv = V*(1 - S);
				double qv = V*(1 - S*f);
				double tv = V*(1 - S*(1 - f));
				switch (i)
				{
						// Red is the dominant color

					case 0:
						R = V;
						G = tv;
						B = pv;
						break;

						// Green is the dominant color

					case 1:
						R = qv;
						G = V;
						B = pv;
						break;
					case 2:
						R = pv;
						G = V;
						B = tv;
						break;

						// Blue is the dominant color

					case 3:
						R = pv;
						G = qv;
						B = V;
						break;
					case 4:
						R = tv;
						G = pv;
						B = V;
						break;

						// Red is the dominant color

					case 5:
						R = V;
						G = pv;
						B = qv;
						break;

						// Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

					case 6:
						R = V;
						G = tv;
						B = pv;
						break;
					case -1:
						R = V;
						G = pv;
						B = qv;
						break;

						// The color is not defined, we should throw an error.

					default:
						//LFATAL("i Value error in Pixel conversion, Value is %d", i);
						R = G = B = V; // Just pretend its black/white
						break;
				}
			}
			r = Clamp((int) (R*255.0));
			g = Clamp((int) (G*255.0));
			b = Clamp((int) (B*255.0));
		}

		/// <summary>
		/// Clamp a value to 0-255
		/// </summary>
		private int Clamp(int i)
		{
			if (i < 0) return 0;
			if (i > 255) return 255;
			return i;
		}
	}
}