namespace Mandelbrot.Core
{
	using System;
	using System.Threading;
	using MassTransit.Grid;

	public class GenerateMandelbrotTask :
		IDistributedTask<GenerateMandelbrotTask, GenerateRow, RowGenerated>,
		IMandelbrotResult
	{
		private readonly int[] _data;
		private readonly int _height;
		private readonly int _iterationLimit;

		private readonly double _offsetX;
		private readonly double _offsetY;
		private readonly double _sampleHeight;
		private readonly double _sampleWidth;
		private readonly int _width;

		private Action<GenerateMandelbrotTask> _completed = x => { };
		private int _rowsCompleted;

		public GenerateMandelbrotTask(int width, int height, int iterationLimit, double offsetX, double offsetY, double sampleWidth, double sampleHeight)
		{
			_offsetX = offsetX;
			_offsetY = offsetY;
			_sampleHeight = sampleHeight;
			_sampleWidth = sampleWidth;
			_iterationLimit = iterationLimit;

			_height = Math.Min(height, GetPerfectHeight(width));
			_width = Math.Min(width, GetPerfectWidth(_height));

			_data = new int[_width*_height];
		}

		public GenerateMandelbrotTask(int width, int height, int iterationLimit)
			: this(width, height, iterationLimit, -2.1, -1.25, 3.2, 2.5)
		{
		}

		public double OffsetX
		{
			get { return _offsetX; }
		}

		public double OffsetY
		{
			get { return _offsetY; }
		}

		public double SampleHeight
		{
			get { return _sampleHeight; }
		}

		public double SampleWidth
		{
			get { return _sampleWidth; }
		}

		public int IterationLimit
		{
			get { return _iterationLimit; }
		}

		public int SubTaskCount
		{
			get { return _height; }
		}

		public GenerateRow GetSubTaskInput(int subTaskId)
		{
			return new GenerateRow
			       	{
			       		Width = _width,
			       		Height = _height,
			       		IterationLimit = _iterationLimit,
			       		Row = subTaskId,
			       		OffsetX = _offsetX,
			       		OffsetY = _offsetY,
			       		SampleWidth = _sampleWidth,
			       		SampleHeight = _sampleHeight,
			       	};
		}

		public void DeliverSubTaskOutput(int subTaskId, RowGenerated output)
		{
			if (subTaskId < _height)
			{
				Array.Copy(output.Data, 0, _data, subTaskId*_width, _width);
				int count = Interlocked.Increment(ref _rowsCompleted);
				if (count == _height)
					_completed(this);
			}
		}

		public void NotifySubTaskException(int subTaskId, Exception ex)
		{
			throw new NotImplementedException();
		}

		public void WhenCompleted(Action<GenerateMandelbrotTask> action)
		{
			_completed += action;
		}

		public int Height
		{
			get { return _height; }
		}

		public int Width
		{
			get { return _width; }
		}

		public int[] Data
		{
			get { return _data; }
		}

		public int GetPerfectHeight(int width)
		{
			return (int) ((_sampleHeight*width)/_sampleWidth);
		}

		public int GetPerfectWidth(int height)
		{
			return (int) ((_sampleWidth*height)/_sampleHeight);
		}
	}
}