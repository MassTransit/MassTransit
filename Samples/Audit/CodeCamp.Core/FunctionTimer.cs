namespace CodeCamp.Core
{
	using System;

	public class FunctionTimer
	{
		private readonly DateTime _start;

		public FunctionTimer()
		{
			_start = DateTime.Now;
		}

		public TimeSpan ElapsedTime
		{
			get { return DateTime.Now - _start; }
		}
	}
}