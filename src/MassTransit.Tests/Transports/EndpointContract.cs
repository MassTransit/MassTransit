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
namespace MassTransit.Tests.Transports
{
    using System;
    using System.Transactions;
    using Configuration;
    using MassTransit.Transports;
    using NUnit.Framework;

    public abstract class EndpointContract<TEndpointFactory> where TEndpointFactory : IEndpointFactory
    {
        IEndpoint _ep;
        IEndpointResolver _endpointResolver;
        public IObjectBuilder ObjectBuilder { get; set; }
        public Uri Address { get; set; }
        public Action<Uri> VerifyMessageIsInQueue { get; set; }
        public Action<Uri> VerifyMessageIsNotInQueue { get; set; }

        [SetUp]
        public void SetUp()
        {
            _endpointResolver = EndpointResolverConfigurator.New(c =>
            {
                c.RegisterTransport<TEndpointFactory>();
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


        [Test]
        public void While_writing_it_should_perisist_on_complete()
        {
            using (TransactionScope trx = new TransactionScope())
            {
                _ep.Send(new DeleteMessage());
                trx.Complete();
            }

            VerifyMessageIsInQueue(Address);
        }

        [Test]
        public void While_writing_it_should_perisist_even_on_rollback()
        {
            using (TransactionScope trx = new TransactionScope())
            {
                _ep.Send(new DeleteMessage());
                //no complete
            }

            VerifyMessageIsInQueue(Address);
        }

        //outside transaction
        [Test]
        public void While_writing_it_should_persist()
        {
            _ep.Send(new DeleteMessage());

            VerifyMessageIsInQueue(Address);
        }
    }
}