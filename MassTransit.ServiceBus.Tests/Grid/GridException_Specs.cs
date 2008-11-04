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
namespace MassTransit.ServiceBus.Tests.Grid
{
    using System;
    using System.Threading;
    using log4net;
    using MassTransit.Grid;
    using MassTransit.ServiceBus.Internal;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class When_a_worker_throws_an_exception :
        GridContextSpecification
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(When_a_worker_throws_an_exception));

        private FactorLongNumbersTask _factorLongNumbers;
        private ManualResetEvent _complete;
        private ManualResetEvent _fault;

        protected override void Before_each()
        {
            base.Before_each();

            _factorLongNumbers = new FactorLongNumbersTask();

            _factorLongNumbers.Add(27);

            _complete = new ManualResetEvent(false);
            _fault = new ManualResetEvent(false);

            _factorLongNumbers.WhenCompleted(x => _complete.Set());
            _factorLongNumbers.WhenExceptionOccurs((t, s, e) =>
                                                       {
                                                           _log.Error("Worker Failed: ", e);
                                                           _fault.Set();
                                                       });
        }

        [Test]
        public void I_want_to_be_able_to_define_a_distributed_task_and_have_it_processed()
        {
            RemoteBus.Subscribe<SubTaskWorker<ExceptionalWorker, FactorLongNumber, LongNumberFactored>>();

            var distributedTaskController =
                new DistributedTaskController<FactorLongNumbersTask, FactorLongNumber, LongNumberFactored>(RemoteBus, Container.Resolve<IEndpointResolver>(), _factorLongNumbers);

            distributedTaskController.Start();

            Assert.That(_fault.WaitOne(TimeSpan.FromSeconds(10), true), Is.True, "Timeout waiting for distributed task to fail");
            Assert.That(_complete.WaitOne(TimeSpan.Zero, false), Is.False, "Task should not have completed");
        }
    }
}