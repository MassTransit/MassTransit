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
namespace MassTransit.Testing.Subjects
{
    using System;
    using System.Threading.Tasks;
    using Scenarios;


    public class HandlerTestSubjectImpl<TScenario, TSubject> :
        HandlerTestSubject<TSubject>
        where TSubject : class
        where TScenario : ITestScenario
    {
        readonly MessageHandler<TSubject> _handler;
        readonly ReceivedMessageListImpl<TSubject> _received;
        bool _disposed;
        ConnectHandle _unsubscribe;

        public HandlerTestSubjectImpl(MessageHandler<TSubject> handler)
        {
            _handler = handler;
            _received = new ReceivedMessageListImpl<TSubject>();
        }

        public IReceivedMessageList<TSubject> Received
        {
            get { return _received; }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Prepare(TScenario scenario)
        {
            _unsubscribe = scenario.Bus.ConnectHandler<TSubject>(HandleMessage);
        }

        void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                if (_unsubscribe != null)
                {
                    _unsubscribe.Dispose();
                    _unsubscribe = null;
                }

                _received.Dispose();
            }

            _disposed = true;
        }

        async Task HandleMessage(ConsumeContext<TSubject> context)
        {
            var received = new ObservedReceivedMessage<TSubject>(context);

            try
            {
                await _handler(context);
            }
            catch (Exception ex)
            {
                received.SetException(ex);
            }
            finally
            {
                _received.Add(received);
            }
        }
    }
}