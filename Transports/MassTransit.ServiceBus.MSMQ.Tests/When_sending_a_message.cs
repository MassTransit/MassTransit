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
    using System.Transactions;
    using Exceptions;
    using MSMQ.Tests.Messages;
    using NUnit.Framework;

    [TestFixture]
    public class When_sending_a_message
    {
        [Test]
        [ExpectedException(typeof(EndpointException))]
        public void To_A_Transactional_Queue_Without_a_transaction()
        {
            string uri = "msmq://localhost/mt_client_tx";
            MsmqEndpoint ep = new MsmqEndpoint(uri);
            QueueTestContext.ValidateAndPurgeQueue(ep.QueuePath, true);

            ep.Send(new DeleteMessage());
        }

        [Test]
        public void To_A_Transactional_Queue_With_a_transaction()
        {
            string uri = "msmq://localhost/mt_client_tx";
            MsmqEndpoint ep = new MsmqEndpoint(uri);
            QueueTestContext.ValidateAndPurgeQueue(ep.QueuePath, true);
         
            using(TransactionScope tr = new TransactionScope())
            {
                ep.Send(new DeleteMessage());
                tr.Complete();
            }

            QueueTestContext.VerifyMessageInQueue(ep.QueuePath, new DeleteMessage());
        }
    }
}