namespace MassTransit.Tests.Subscriptions
{
	using System;
	using System.Threading;
	using MassTransit.Pipeline.Inspectors;
	using MassTransit.Pipeline.Sinks;
	using TextFixtures;
	using MassTransit.Transports;
	using NUnit.Framework;
	using Magnum.TestFramework;

	[TestFixture]
	public class Adding_a_correlated_subscription_via_the_subscription_client: //Not bdd style.
		SubscriptionServiceTestFixture<LoopbackEndpointFactory>
	{
        
		[Test]
		public void Should_properly_register_the_consumers_for_each_endpoint()
		{
			var firstSub = new FirstSybsystem();
			var secondSub = new SecondSubsystem();
			LocalBus.Subscribe(firstSub);
			LocalBus.Subscribe(secondSub);
			Thread.Sleep(2500);
			var inspector = new CorrelatedRouterPipelineInspector();
			RemoteBus.OutboundPipeline.Inspect(inspector);
			inspector.PipelineHasRightRoutings.ShouldBeTrue("OutboundPipeline on publisher should contains 'CorrelatedMessageSinkRouter' for IncomingMessage with 'FirstSybsystemCorrelationId' and 'SecondSubsystemCorrelationId' correlations id's.");
		}


		public class CorrelatedRouterPipelineInspector : PipelineInspectorBase<CorrelatedRouterPipelineInspector>
		{
			private bool firstSubSystemRouteDefined;
			private bool secondSubSystemRouteDefined;

			public virtual bool PipelineHasRightRoutings
			{
				get { return firstSubSystemRouteDefined && secondSubSystemRouteDefined; }
			}

			public bool Inspect<TMessage, TKey>(CorrelatedMessageSinkRouter<TMessage, TKey> sink)
				where TMessage : class, CorrelatedBy<TKey>
			{

				if (sink.CorrelationId.ToString() == "FirstSybsystemCorrelationId")
				{
					firstSubSystemRouteDefined = true;   
				}

				if (sink.CorrelationId.ToString() == "SecondSubsystemCorrelationId")
				{
					secondSubSystemRouteDefined = true;
				}

				return true;
			}
		}

		public class FirstSybsystem : Consumes<IncomingMessage>.For<string>
		{
			public void Consume(IncomingMessage message)
			{
			}

			public string CorrelationId
			{
				get { return "FirstSybsystemCorrelationId"; }
			}
		}

		public class SecondSubsystem : Consumes<IncomingMessage>.For<string>
		{
			public void Consume(IncomingMessage message)
			{
			}

			public string CorrelationId
			{
				get { return "SecondSubsystemCorrelationId"; }
			}
		}

		[Serializable]
		public class IncomingMessage : CorrelatedBy<string>
		{
			private string correlationId;

			protected IncomingMessage()
			{
			}

			public IncomingMessage(string correlationId)
			{
				this.correlationId = correlationId;
			}

			public virtual string CorrelationId
			{
				get { return correlationId; }
				protected set { correlationId = value; }
			}
		}
	}
}