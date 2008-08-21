namespace MassTransit.Grid.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Text;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

	[TestFixture]
	public class When_factoring_integer_values
	{
		[Test]
		public void Factor_4()
		{
			IntegerFactorizer factorizer = new IntegerFactorizer();

			IList<long> factors = factorizer.Factor(4);

			Assert.That(factors.Count, Is.EqualTo(2));
			Assert.That(factors[0], Is.EqualTo(2));
			Assert.That(factors[1], Is.EqualTo(2));
		}

		[Test]
		public void Factor_21()
		{
			IntegerFactorizer factorizer = new IntegerFactorizer();

			IList<long> factors = factorizer.Factor(21);

			Assert.That(factors.Count, Is.EqualTo(2));
			Assert.That(factors[0], Is.EqualTo(3));
			Assert.That(factors[1], Is.EqualTo(7));
		}

		[Test]
		public void Factor_31()
		{
			IntegerFactorizer factorizer = new IntegerFactorizer();

			IList<long> factors = factorizer.Factor(31);

			Assert.That(factors.Count, Is.EqualTo(1));
			Assert.That(factors[0], Is.EqualTo(31));
		}

		[Test]
		public void Factor_27()
		{
			IntegerFactorizer factorizer = new IntegerFactorizer();

			IList<long> factors = factorizer.Factor(27);

			Assert.That(factors.Count, Is.EqualTo(3));
			Assert.That(factors[0], Is.EqualTo(3));
			Assert.That(factors[1], Is.EqualTo(3));
			Assert.That(factors[2], Is.EqualTo(3));
		}

		[Test]
		public void Factor_Huge_Number()
		{
			IntegerFactorizer factorizer = new IntegerFactorizer();

			Random random = new Random();

			long value = (long)(random.NextDouble()*10000000000000);

			IList<long> factors = factorizer.Factor(value);

			Assert.That(factors.Count, Is.GreaterThan(0));

			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("Factoring {0} into ", value);

			foreach (long factor in factors)
			{
				sb.AppendFormat("{0:f0} ", factor);
			}

			Debug.WriteLine(sb.ToString());
		}
	}
}