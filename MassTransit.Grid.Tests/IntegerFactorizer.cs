namespace MassTransit.Grid.Tests
{
	using System.Collections.Generic;

	public class IntegerFactorizer
	{
		public IList<long> Factor(long value)
		{
			List<long> factors = new List<long>();

			for (int factor = 2; factor <= value; factor++)
			{
				if (factor == value)
				{
					factors.Add(value);
					break;
				}

				if (value%factor != 0) 
					continue;

				factors.Add(factor);

				value /= factor;

				factor = 1;
			}

			return factors;
		}
	}
}