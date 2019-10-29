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
namespace MassTransit.Tests.Pipeline
{
    using System.Threading.Tasks;
    using Internals.Extensions;
    using TestFramework.Messages;


    public class OneMessageConsumer :
        IConsumer<MessageA>
    {
        readonly TaskCompletionSource<MessageA> _completed;

        public OneMessageConsumer()
        {
            _completed = TaskCompletionSourceFactory.New<MessageA>();
        }

        public OneMessageConsumer(TaskCompletionSource<MessageA> completed)
        {
            _completed = completed;
        }

        public Task<MessageA> Task
        {
            get { return _completed.Task; }
        }

        public async Task Consume(ConsumeContext<MessageA> context)
        {
            _completed.TrySetResult(context.Message);
        }
    }
}
