using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoadTestClient
{
	using System;
	using Magnum;

	class Program
	{
		static void Main(string[] args)
		{
			//var target = new Uri("http://localhost/loadtest/Actors/StockQuoteRequest");
			var target = new Uri("http://localhost:49590/Actors/StockQuoteRequest");

			var actors = new List<SimulateRequestActor>();
			for (int i = 0; i < 1000; i++)
			{
				actors.Add(new SimulateRequestActor(target));
			}

			Console.ReadLine();

			actors.Each(x => x.Stop());

			Console.ReadLine();
		}
	}
}
