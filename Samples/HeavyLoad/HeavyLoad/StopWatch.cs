namespace HeavyLoad
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Text;

	public class StopWatch
	{
		private readonly List<CheckPoint> _marks = new List<CheckPoint>();
		private DateTime _start;
		private DateTime _stop;
	    private Stopwatch _stopwatch;

		public void Start()
		{
			_start = DateTime.Now;
		    _stopwatch = Stopwatch.StartNew();
		}

		public void Stop()
		{
			_stop = DateTime.Now;
		    _stopwatch.Stop();
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendFormat("Started at {0}\n", _start);

			foreach (CheckPoint mark in _marks)
			{
				mark.ToString(sb);
			}

			sb.AppendFormat("Finished at {0}\n", _stop);
			sb.AppendFormat("Total elapsed time: {0}\n", _stopwatch.Elapsed);

			return sb.ToString();
		}

		public CheckPoint Mark(string description)
		{
			CheckPoint point = new CheckPoint(description);

			_marks.Add(point);

			return point;
		}
	}

	public class CheckPoint
	{
		private readonly string _description;
		private int _operationCount = 1;
	    private readonly Stopwatch _stopwatch;

		public CheckPoint(string description)
		{
			_description = description;
		    _stopwatch = Stopwatch.StartNew();
		}

		public void ToString(StringBuilder sb)
		{
			sb.AppendFormat("  {0}{1}", _description, Environment.NewLine);

			if (_operationCount > 1)
			{
				sb.AppendFormat("    {0} messages in {1:#.##} seconds{2}", _operationCount, _stopwatch.Elapsed.TotalSeconds, Environment.NewLine);
                sb.AppendFormat("      {0:#,###.##} messages/second", _operationCount / _stopwatch.Elapsed.TotalSeconds);
			}

			sb.AppendLine();
		}

		public void Complete(int operationCount)
		{
		    _stopwatch.Stop();

			_operationCount = operationCount;
		}
	}
}