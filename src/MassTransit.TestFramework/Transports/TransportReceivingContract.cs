// Copyright 2007-2011 The Apache Software Foundation.
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
namespace MassTransit.TestFramework.Transports
{
    using System;
    using Configuration;
    using MassTransit.Transports;
    using Messages;
    using NUnit.Framework;
    using Rhino.Mocks;

    public abstract class TransportReceivingContract<TTransportFactory>
        where TTransportFactory : ITransportFactory
    {
        IEndpoint _ep;
        IEndpointResolver _endpointResolver;
        public IObjectBuilder ObjectBuilder { get; set; }
        public Uri Address { get; set; }
        public Action<Uri> VerifyMessageIsNotInQueue { get; set; }

        protected TransportReceivingContract(Uri uri)
        {
            Address = uri;
        }


        [SetUp]
        public void SetUp()
        {
            ObjectBuilder = MockRepository.GenerateStub<IObjectBuilder>();
            _endpointResolver = EndpointResolverConfigurator.New(c =>
            {
                c.RegisterTransport<TTransportFactory>();
                c.SetObjectBuilder(ObjectBuilder);
            });
            _ep = _endpointResolver.GetEndpoint(Address);
        }

        [TearDown]
        public void TearDown()
        {
            _ep.Dispose();
            _ep = null;
        }


        public void VerifyMessageIsInQueue(IEndpoint ep)
        {
            ep.ShouldContain<DeleteMessage>();
        }
    }
}