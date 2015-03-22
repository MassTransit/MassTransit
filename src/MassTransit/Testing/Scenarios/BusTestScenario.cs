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
namespace MassTransit.Testing.Scenarios
{
    using System;
    using System.Threading;


    /// <summary>
    /// A bus test scenario includes a bus only, with no receiving endpoints
    /// </summary>
    public class BusTestScenario :
        IBusTestScenario
    {
        readonly IBusControl _busControl;
        readonly CancellationToken _cancellationToken;
        readonly PublishedMessageList _published;
        readonly ReceivedMessageList _received;
        readonly ObservedSentMessageList _sent;
        readonly ReceivedMessageList _skipped;
        readonly TimeSpan _timeout;
        readonly CancellationTokenSource _tokenSource;
        BusHandle _busHandle;

        public BusTestScenario(TimeSpan timeout, IBusControl busControl)
        {
            _timeout = timeout;
            _busControl = busControl;

            _received = new ReceivedMessageList(timeout);
            _sent = new ObservedSentMessageList(timeout);
            _skipped = new ReceivedMessageList(timeout);
            _published = new PublishedMessageList(timeout);

            _tokenSource = new CancellationTokenSource(timeout);
            _cancellationToken = _tokenSource.Token;

            _busHandle = _busControl.Start();
        }

        public virtual ISendEndpoint SubjectSendEndpoint
        {
            get { return Bus.GetSendEndpoint(new Uri("loopback://localhost/input_queue")).Result; }
        }

        public ISentMessageList Sent
        {
            get { return _sent; }
        }

        public IReceivedMessageList Skipped
        {
            get { return _skipped; }
        }

        public CancellationToken CancellationToken
        {
            get { return _cancellationToken; }
        }

        public TimeSpan Timeout
        {
            get { return _timeout; }
        }

        public void Cancel()
        {
            _tokenSource.Cancel();
        }

        public IPublishedMessageList Published
        {
            get { return _published; }
        }

        public IReceivedMessageList Received
        {
            get { return _received; }
        }

        public virtual void Dispose()
        {
            if (_busHandle != null)
            {
                _busHandle.Dispose();
                _busHandle = null;
            }
        }

        public virtual IBus Bus
        {
            get { return _busControl; }
        }
    }
}