// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Testing.Scenarios
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MessageObservers;
    using Observers;


    /// <summary>
    /// A bus test scenario includes a bus only, with no receiving endpoints
    /// </summary>
    public class BusTestScenario :
        IBusTestScenario
    {
        readonly IBusControl _busControl;
        readonly PublishedMessageList _published;
        readonly ReceivedMessageList _skipped;
        readonly Task<ISendEndpoint> _subjectSendEndpoint;
        readonly TimeSpan _timeout;
        readonly CancellationTokenSource _tokenSource;
        Task<BusHandle> _busHandle;

        public BusTestScenario(TimeSpan timeout, IBusControl busControl)
        {
            _timeout = timeout;
            _busControl = busControl;

            Received = new ReceivedMessageList(timeout);
            _skipped = new ReceivedMessageList(timeout);
            _published = new PublishedMessageList(timeout);

            _tokenSource = new CancellationTokenSource(timeout);
            CancellationToken = _tokenSource.Token;


            var testSendObserver = new TestSendObserver(timeout);
            Sent = testSendObserver.Messages;

            _subjectSendEndpoint = GetSendEndpoint(testSendObserver);

            var consumeObserver = new TestConsumeObserver(timeout);
            Received = consumeObserver.Messages;
            busControl.ConnectConsumeObserver(consumeObserver);

            _busHandle = _busControl.StartAsync();
        }

        public virtual Task<ISendEndpoint> SubjectSendEndpoint => _subjectSendEndpoint;

        public ISentMessageList Sent { get; }

        public IReceivedMessageList Skipped => _skipped;

        public CancellationToken CancellationToken { get; }

        public TimeSpan Timeout => _timeout;

        public void Cancel()
        {
            _tokenSource.Cancel();
        }

        public IPublishedMessageList Published => _published;

        public IReceivedMessageList Received { get; }

        public virtual async Task DisposeAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_busHandle != null)
            {
                var busHandle = await _busHandle.ConfigureAwait(false);
                await busHandle.StopAsync(_timeout).ConfigureAwait(false);
                _busHandle = null;
            }
        }

        public virtual IBus Bus => _busControl;

        async Task<ISendEndpoint> GetSendEndpoint(TestSendObserver testSendObserver)
        {
            var endpoint = await _busControl.GetSendEndpoint(new Uri("loopback://localhost/input_queue")).ConfigureAwait(false);

            endpoint.ConnectSendObserver(testSendObserver);

            return endpoint;
        }
    }
}