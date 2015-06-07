// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Testing.TestDecorators
{
    using System;
    using System.Threading.Tasks;
    using Pipeline;


    public class TestConsumeObserver :
        IConsumeObserver
    {
        readonly ReceivedMessageList _messages;

        public TestConsumeObserver(TimeSpan timeout)
        {
            _messages = new ReceivedMessageList(timeout);
        }

        public IReceivedMessageList Messages
        {
            get { return _messages; }
        }

        async Task IConsumeObserver.PreConsume<T>(ConsumeContext<T> context)
        {
        }

        async Task IConsumeObserver.PostConsume<T>(ConsumeContext<T> context)
        {
            _messages.Add(context);
        }

        async Task IConsumeObserver.ConsumeFault<T>(ConsumeContext<T> context, Exception exception)
        {
            _messages.Add(context, exception);
        }
    }
}