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
    using System.Threading.Tasks;
    using TestActions;
    using Util;


    public abstract class TestInstance<TScenario>
        where TScenario : ITestScenario
    {
        readonly IList<ITestAction<TScenario>> _actions;
        readonly TScenario _scenario;
        bool _disposed;

        protected TestInstance(TScenario scenario, IList<ITestAction<TScenario>> actions)
        {
            _scenario = scenario;
            _actions = actions;
        }

        public IReceivedMessageList Received => _scenario.Received;

        public ISentMessageList Sent => _scenario.Sent;

        public IPublishedMessageList Published => _scenario.Published;

        public IReceivedMessageList Skipped => _scenario.Skipped;

        public TScenario Scenario => _scenario;

        public void Execute()
        {
            TaskUtil.Await(ExecuteAsync);
        }

        public Task ExecuteAsync()
        {
            return ExecuteTestActions();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                _scenario.Dispose();
            }

            _disposed = true;
        }

        async Task ExecuteTestActions()
        {
            foreach (var action in _actions)
            {
                if (_scenario.CancellationToken.IsCancellationRequested)
                    throw new TaskCanceledException("The test was cancelled");

                await action.Act(_scenario, _scenario.CancellationToken).ConfigureAwait(false);
            }
        }
    }
}