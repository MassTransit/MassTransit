namespace Mandelbrot.Core
{
	using System;
	using System.Threading;
	using MassTransit.Grid;

	public class GenerateMandelbrotTask :
		IDistributedTask<GenerateMandelbrotTask, GenerateRow, RowGenerated>
	{
		private readonly int _height;
		private readonly int _iterationLimit;
		private readonly int _width;

		private readonly byte[] _data;
		private Action<GenerateMandelbrotTask> _completed = (x) => { };
		private int _rowsCompleted;

		public GenerateMandelbrotTask(int width, int height, int iterationLimit)
		{
			_width = width;
			_height = height;
			_iterationLimit = iterationLimit;

			_data = new byte[width * height];
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
						Row = subTaskId
			       	};
		}

		public void DeliverSubTaskOutput(int subTaskId, RowGenerated output)
		{
			if(subTaskId < _height)
			{
				Array.Copy(output.Data, 0, _data, subTaskId*_width, _width);
				int count = Interlocked.Increment(ref _rowsCompleted);
				if (count == _height)
					_completed(this);
			}
		}

		public void NotifySubTaskException(int subTaskId, Exception ex)
		{
			throw new System.NotImplementedException();
		}

		public void WhenCompleted(Action<GenerateMandelbrotTask> action)
		{
			_completed += action;
		}
	}

	[Serializable]
	public class GenerateRow
	{
		public int Width { get; set; }

		public int Height { get; set; }

		public int Row { get; set; }

		public int IterationLimit { get; set; }
	}

	[Serializable]
	public class RowGenerated
	{
		public byte[] Data { get; set; }
	}
}