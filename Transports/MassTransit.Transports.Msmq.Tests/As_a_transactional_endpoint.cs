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
namespace MassTransit.Transports.Msmq.Tests
{
    using System;
    using System.Transactions;
    using Exceptions;
    using MassTransit.Tests;
    using NUnit.Framework;


    //as a transactional endpoint
    [TestFixture]
    public class When_in_a_transaction
    {
        private MsmqEndpoint _ep;

        [SetUp]
        public void SetUp()
        {
            MsmqUtilities.ValidateAndPurgeQueue(".\\private$\\mt_client_tx", true);
            _ep = new MsmqEndpoint("msmq://localhost/mt_client_tx");
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


            _ep.VerifyMessageInTransactionalQueue<DeleteMessage>();
        }

        [Test]
        public void While_writing_it_should_not_perisist_on_rollback()
        {
            using (TransactionScope trx = new TransactionScope())
            {
                _ep.Send(new DeleteMessage());
                //no complete
            }

            _ep.VerifyMessageNotInTransactionalQueue();
        }


    }

    [TestFixture]
    public class When_outside_a_transaction
    {
        private MsmqEndpoint _ep;

        [SetUp]
        public void SetUp()
        {
            MsmqUtilities.ValidateAndPurgeQueue(".\\private$\\mt_client_tx", true);
            _ep = new MsmqEndpoint("msmq://localhost/mt_client_tx");
        }

        [TearDown]
        public void TearDown()
        {
            _ep.Dispose();
            _ep = null;
        }


        [Test]
        public void It_should_auto_enlist_a_transaction_and_persist()
        {
            _ep.Send(new DeleteMessage());
            _ep.VerifyMessageNotInTransactionalQueue();
        }

    }
}