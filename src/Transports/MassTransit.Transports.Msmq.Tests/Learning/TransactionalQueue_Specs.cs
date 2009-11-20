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
namespace MassTransit.Transports.Msmq.Tests.Learning
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Transactions;
    using NUnit.Framework;

    [TestFixture, Category("Integration")]
    public class When_reading_from_a_transactional_queue
    {
        private readonly ManualResetEvent _txCompleted = new ManualResetEvent(false);

        [Test]
        public void A_transaction_should_be_dependent_upon_the_worker_thread_committing()
        {
            Thread thx = new Thread(ThreadProc);

            Debug.WriteLine(string.Format("{0} Opening transaction", DateTime.Now));

            using ( TransactionScope ts = new TransactionScope())
            {
                DependentTransaction dts = Transaction.Current.DependentClone(DependentCloneOption.BlockCommitUntilComplete);

                Debug.WriteLine(string.Format("{0} Starting thread", DateTime.Now));

                thx.Start(dts);

                Debug.WriteLine(string.Format("{0} Completing outer transaction", DateTime.Now));

                ts.Complete();

                Debug.WriteLine(string.Format("{0} Exiting transaction scope", DateTime.Now));
            }

            Debug.WriteLine(string.Format("{0} Verifying transaction not yet complete", DateTime.Now));


            Assert.That(_txCompleted.WaitOne(0, false), Is.True, "It seems that the original thread blocks until the dependent transaction is completed.");
        }

        public void ThreadProc(object tsObject)
        {
            DependentTransaction dts = (DependentTransaction)tsObject;

            Debug.WriteLine(string.Format("{0} Opening dependent transaction", DateTime.Now));

            using (TransactionScope ts = new TransactionScope(dts))
            {
                Debug.WriteLine(string.Format("{0} Going to sleep", DateTime.Now));

                Thread.Sleep(10000);

                Debug.WriteLine(string.Format("{0} Completing dependent transaction", DateTime.Now));

                ts.Complete();

                Debug.WriteLine(string.Format("{0} Dependent transaction completed, setting event", DateTime.Now));

                _txCompleted.Set();
            }

            Debug.WriteLine(string.Format("{0} Completing outer transaction", DateTime.Now));

            dts.Complete();

            Debug.WriteLine(string.Format("{0} Thread Exiting", DateTime.Now));

        }
    }
}