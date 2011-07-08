// Copyright 2007-2010 The Apache Software Foundation.
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
namespace MassTransit.Transports.Msmq.Tests.Learning
{
    using System;
    using System.Messaging;
    using System.Transactions;
    using Magnum.Extensions;
    using NUnit.Framework;

    //TODO: Move to an Assumptions test section like we did for RabbitMQ
    [TestFixture, Category("Integration")]
    public class When_receiving_from_a_transactional_queue
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            if (!MessageQueue.Exists(_queueName))
                MessageQueue.Create(_queueName, true);

            using(var purgeq = new MessageQueue(_queueName))
                purgeq.Purge();
            

            _queue = new MessageQueue(_queueName, false, true, QueueAccessMode.SendAndReceive);
            

            _firstMsg = new Message {Label = 0.Days().FromUtcNow().ToString()};
            _secondMsg = new Message {Label = 1.Days().FromUtcNow().ToString()};

            _queue.Send(_firstMsg, MessageQueueTransactionType.Single);
            _queue.Send(_secondMsg, MessageQueueTransactionType.Single);
        }

        [TearDown]
        public void Teardown()
        {
            try
            {
                if(_queue != null)
                    _queue.Close();
            }
            finally
            {
                if(_queue != null)
                    _queue.Dispose();
                _queue = null;
            }
        }

        #endregion

        private MessageQueue _queue;
        private Message _firstMsg;
        private Message _secondMsg;
        private const string _queueName = @".\private$\test_transactionalqueue";

        [Test]
        public void A_transaction_in_progress_should_not_block_the_queue()
        {
            try
            {
                _queue.WithinTransaction((q, t) =>
                                             {
                                                 Message received = q.Receive(t);

                                                 Assert.IsNotNull(received);

                                                 Assert.AreEqual(received.Label, _firstMsg.Label);

                                                 _queue.WithinTransaction((q2, t2) =>
                                                                              {
                                                                                  Message second = q2.Receive(t);

                                                                                  Assert.IsNotNull(second);

                                                                                  Assert.AreEqual(second.Label, _secondMsg.Label);
                                                                              });

                                                 throw new AssertionException("Expect this");
                                             });
            }
            catch
            {
            }

            _queue.WithinTransaction((q, t) =>
                                         {
                                             Message received = q.Receive(t);

                                             Assert.IsNotNull(received);

                                             Assert.AreEqual(received.Label, _firstMsg.Label);
                                         });
        }

        [Test]
        public void The_enumerator_should_skip_over_messages_that_are_being_processed()
        {
            TimeSpan timeout = 10.Seconds();

            using (MessageEnumerator enumerator = _queue.GetMessageEnumerator2())
            {
                while (enumerator.MoveNext(timeout))
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        Message received = enumerator.RemoveCurrent(timeout, MessageQueueTransactionType.Automatic);

                        Assert.IsNotNull(received);

                        Assert.AreEqual(received.Label, _firstMsg.Label);

                        using (MessageEnumerator enumerator2 = _queue.GetMessageEnumerator2())
                        {
                            while (enumerator2.MoveNext(timeout))
                            {
                                using (TransactionScope scope2 = new TransactionScope(TransactionScopeOption.RequiresNew))
                                {
                                    Message received2 = enumerator2.RemoveCurrent(timeout, MessageQueueTransactionType.Automatic);

                                    Assert.IsNotNull(received2);

                                    Assert.AreEqual(received2.Label, _secondMsg.Label);

                                    scope2.Complete();
                                }
                            }
                        }

                        scope.Complete();
                    }
                }
            }
        }
    }

    [TestFixture, Category("Integration")]
    public class When_receiving_from_a_queue
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            if (!MessageQueue.Exists(_queueName))
                MessageQueue.Create(_queueName, true);

            _queue = new MessageQueue(_queueName, false, true, QueueAccessMode.SendAndReceive);
            _queue.Purge();

            _firstMsg = new Message { Label = 0.Days().FromUtcNow().ToString() };
            _secondMsg = new Message { Label = 1.Days().FromUtcNow().ToString() };

            _queue.Send(_firstMsg);
            _queue.Send(_secondMsg);
        }

        [TearDown]
        public void Teardown()
        {
            try
            {
                _queue.Close();
            }
            finally
            {
                _queue.Dispose();
                _queue = null;
            }
        }

        #endregion

        private MessageQueue _queue;
        private Message _firstMsg;
        private Message _secondMsg;
        private const string _queueName = @".\private$\mt_client";





        [Test]
        public void Ids()
        {
            Message once;
            Message twice;

            using (MessageEnumerator enumerator = _queue.GetMessageEnumerator2())
            {
                if(enumerator.MoveNext())
                {
                    once = enumerator.Current;
                    enumerator.MoveNext();
                    twice = enumerator.RemoveCurrent();

                	Assert.AreNotSame(once.Id, twice.Id);
                    var b = twice.BodyType;
                }
                enumerator.Close();
            }

            
        }
    }
}