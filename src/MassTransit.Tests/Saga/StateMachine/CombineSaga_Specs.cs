// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests.Saga.StateMachine
{
    using System;
    using Magnum;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using MassTransit.Pipeline.Inspectors;
    using MassTransit.Saga;
    using NUnit.Framework;
    using TestFramework;
    using TextFixtures;

    [TestFixture]
    public class CombineSaga_Specs : LoopbackTestFixture
    {
        private ISagaRepository<CombineSaga> _repository;
        private Guid _transactionId;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            _repository = SetupSagaRepository<CombineSaga>();
            _transactionId = CombGuid.Generate();
        }

        [Test]
        public void A_state_machine_should_respond_to_combined_events()
        {
            LocalBus.SubscribeSaga(_repository);

            PipelineViewer.Trace(LocalBus.InboundPipeline);
            PipelineViewer.Trace(LocalBus.OutboundPipeline);

            LocalBus.Publish(new Second {CorrelationId = _transactionId});

            var saga = _repository.ShouldContainSaga(_transactionId, 8.Seconds());
            saga.ShouldBeInState(CombineSaga.Initial);
            saga.Combined.ShouldEqual(2);

            LocalBus.Publish(new First {CorrelationId = _transactionId});

            saga.ShouldBeInState(CombineSaga.Completed);
            saga.Combined.ShouldEqual(3);
        }
    }
}