namespace HeavyLoad
{
	using System;

	[Serializable]
	public class GeneralMessage
	{
		private int[] _values;

		public GeneralMessage()
		{
			_values = new int[64];
		}

		public int[] Values
		{
			get { return _values; }
			set { _values = value; }
		}
	}
}