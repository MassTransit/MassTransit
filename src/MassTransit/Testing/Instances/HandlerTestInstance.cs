// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Testing.Instances
{
    using System.Collections.Generic;
    using Scenarios;
    using Subjects;
    using TestActions;


    public class HandlerTestInstance<TScenario, TMessage> :
        TestInstance<TScenario>,
        HandlerTest<TScenario, TMessage>
        where TMessage : class
        where TScenario : ITestScenario
    {
        readonly HandlerTestSubjectImpl<TScenario, TMessage> _subject;

        bool _disposed;

        public HandlerTestInstance(TScenario scenario, IList<ITestAction<TScenario>> actions, MessageHandler<TMessage> handler)
            : base(scenario, actions)
        {
            _subject = new HandlerTestSubjectImpl<TScenario, TMessage>(handler);
        }

        public void Execute()
        {
            _subject.Prepare(Scenario);

            ExecuteTestActions();
        }

        public HandlerTestSubject<TMessage> Handler
        {
            get { return _subject; }
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
                _subject.Dispose();

            base.Dispose(disposing);

            _disposed = true;
        }
    }
}