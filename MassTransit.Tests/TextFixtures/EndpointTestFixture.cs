// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Tests.TextFixtures
{
	using Configuration;
	using Magnum.Common.DateTimeExtensions;
	using MassTransit.Internal;
	using MassTransit.Subscriptions;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public abstract class EndpointTestFixture<TTransport>
		where TTransport : IEndpoint
	{
		[SetUp]
		public void Setup()
		{
			ObjectBuilder = MockRepository.GenerateMock<IObjectBuilder>();

			ISubscriptionCache subscriptionCache = new LocalSubscriptionCache();
			ObjectBuilder.Stub(x => x.GetInstance<ISubscriptionCache>()).Return(subscriptionCache);

			ITypeInfoCache typeInfoCache = new TypeInfoCache();
			ObjectBuilder.Stub(x => x.GetInstance<ITypeInfoCache>()).Return(typeInfoCache);

			EndpointFactory = EndpointFactoryConfigurator.New(x =>
				{
					x.SetObjectBuilder(ObjectBuilder);
					x.RegisterTransport<TTransport>();
				});
			ObjectBuilder.Stub(x => x.GetInstance<IEndpointFactory>()).Return(EndpointFactory);

			ServiceBusConfigurator.Defaults(x => 
			{
				x.SetObjectBuilder(ObjectBuilder);
				x.SetReceiveTimeout(50.Milliseconds());
			});

			EstablishContext();
		}

		[TearDown]
		public void Teardown()
		{
			TeardownContext();

			EndpointFactory.Dispose();
			EndpointFactory = null;
		}

		protected IEndpointFactory EndpointFactory { get; private set; }
		protected IObjectBuilder ObjectBuilder { get; private set; }

		protected virtual void EstablishContext()
		{
		}

		protected virtual void TeardownContext()
		{
		}
	}
}