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
    using System.Transactions;
    using Configuration;
    using MassTransit.Transports;
    using Messages;
    using NUnit.Framework;
    using Rhino.Mocks;

    public abstract class TransportContract<TTransportFactory>
        where TTransportFactory : ITransportFactory
    {
        IEndpoint _ep;
        IEndpointCache _endpointCache;
        public IObjectBuilder ObjectBuilder { get; set; }
        public Uri Address { get; set; }
        public Action<Uri> VerifyMessageIsNotInQueue { get; set; }

        protected TransportContract(Uri uri)
        {
            Address = uri;
        }


        [SetUp]
        public void SetUp()
        {
            ObjectBuilder = MockRepository.GenerateStub<IObjectBuilder>();
            _endpointCache = EndpointResolverConfiguratorImpl.New(c =>
            {
                c.AddTransportFactory<TTransportFactory>();
                c.SetObjectBuilder(ObjectBuilder);
            });
            _ep = _endpointCache.GetEndpoint(Address);
        }

        [TearDown]
        public void TearDown()
        {
            _ep.Dispose();
            _ep = null;
        }


        [Test]
        public void While_writing_it_should_perisist_on_complete()
        {
            using (TransactionScope trx = new TransactionScope())
            {
                _ep.Send(new DeleteMessage());
                trx.Complete();
            }

            VerifyMessageIsInQueue(_ep);
        }

        [Test]
        public void While_writing_it_should_perisist_even_on_rollback()
        {
            using (TransactionScope trx = new TransactionScope())
            {
                _ep.Send(new DeleteMessage());
                //no complete
            }

            VerifyMessageIsInQueue(_ep);
        }

        //outside transaction
        [Test]
        public void While_writing_it_should_persist()
        {
            _ep.Send(new DeleteMessage());

            VerifyMessageIsInQueue(_ep);
        }


        public void VerifyMessageIsInQueue(IEndpoint ep)
        {
            ep.ShouldContain<DeleteMessage>();
        }

    }
}