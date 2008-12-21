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
namespace MassTransit.Tests.Subscriptions
{
    using System;
    using Configuration;
    using MassTransit.Internal;
    using MassTransit.Serialization;
    using MassTransit.Subscriptions;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;
    
    using Tests.Messages;
    using Transports;

    [TestFixture]
    public class When_a_handler_unsubscribes_from_the_service_bus
        : Specification
    {
        private IObjectBuilder _builder;

        protected override void Before_each()
        {
			_endpointResolver = EndpointFactoryConfigurator.New(x =>
			{
				x.SetObjectBuilder(_builder);
				x.SetDefaultSerializer<BinaryMessageSerializer>();
				x.RegisterTransport<LoopbackEndpoint>();
			});

            _builder = StrictMock<IObjectBuilder>();
            _endpoint = _endpointResolver.GetEndpoint(_endpointUri);

            _cache = _mocks.DynamicMock<ISubscriptionCache>();

            _bus = new ServiceBus(_endpoint, _builder, _cache, _endpointResolver, new TypeInfoCache());
        }


        private MockRepository _mocks = new MockRepository();
        private IEndpoint _endpoint;
        private ISubscriptionCache _cache;
        private ServiceBus _bus;
        private object _message = new PingMessage();
        private Uri _endpointUri = new Uri("loopback://localhost/test");
        private IEndpointFactory _endpointResolver;


        private static void HandleAllMessages(PingMessage ctx)
        {
        }

        private static bool HandleSomeMessagesPredicate(PingMessage message)
        {
            return true;
        }
    }
}