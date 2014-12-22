// Copyright 2011 Chris Patterson, Dru Sellers
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
namespace MassTransit.AutomatonymousTests
{
    using System;
    using System.Linq;
    using System.Threading;
    using Automatonymous;
    using NUnit.Framework;
    using Saga;


//    public static class TestExtensions
//    {
//        public static TSaga ShouldContainSaga<TSaga>(this ISagaRepository<TSaga> repository, Guid sagaId,
//                                                     TimeSpan timeout)
//            where TSaga : class, ISaga
//        {
//            DateTime giveUpAt = DateTime.Now + timeout;
//
//            while (DateTime.Now < giveUpAt)
//            {
//                TSaga saga = repository.Where(x => x.CorrelationId == sagaId).FirstOrDefault();
//                if (saga != null)
//                    return saga;
//
//                Thread.Sleep(100);
//            }
//
//            return null;
//        }
//
//
//        public static void ShouldNotContainSaga<TSaga>(this ISagaRepository<TSaga> repository, Guid sagaId,
//                                                     TimeSpan timeout)
//            where TSaga : class, ISaga
//        {
//            DateTime giveUpAt = DateTime.Now + timeout;
//
//            while (DateTime.Now < giveUpAt)
//            {
//                TSaga saga = repository.Where(x => x.CorrelationId == sagaId).FirstOrDefault();
//                if (saga == null)
//                    return;
//
//                Thread.Sleep(100);
//            }
//
//            Assert.Fail("The saga instance exists: " + sagaId);
//        }
//
//        public static TSaga ShouldContainSagaInState<TSaga>(this ISagaRepository<TSaga> repository, Guid sagaId,
//                                                            State state, StateMachine<TSaga> machine, TimeSpan timeout)
//            where TSaga : class, SagaStateMachineInstance
//        {
//            DateTime giveUpAt = DateTime.Now + timeout;
//
//            while (DateTime.Now < giveUpAt)
//            {
//                TSaga saga = repository.ShouldContainSaga(sagaId, timeout);
//                if (machine.InstanceStateAccessor.Get(saga).Equals(state))
//                    return saga;
//
//                Thread.Sleep(100);
//            }
//
//            return null;
//        }
//    }
}