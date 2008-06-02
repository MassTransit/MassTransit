namespace MassTransit.ServiceBus.Util
{
	using System.Diagnostics;

	public static class Guard
	{
		public static class Against
		{
			public static bool UseExceptions
			{
				get { return _useExceptions; }
				set { _useExceptions = value; }
			}

			private static bool _useExceptions = true;

			public static void Null(object obj, string message)
			{
				if (UseExceptions)
				{
					if (obj == null)
						throw new PreconditionException(message);
				}
				else
				{
					Trace.Assert(obj != null, "Precondition: " + message);
				}
			}
		}
	}
}