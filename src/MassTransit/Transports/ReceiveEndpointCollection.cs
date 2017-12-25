// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Pipeline.Observables;
    using Util;


    public class ReceiveEndpointCollection :
        IReceiveEndpointCollection
    {
        readonly ConsumeObservable _consumeObservers;
        readonly Dictionary<string, IReceiveEndpointControl> _endpoints;
        readonly Dictionary<string, HostReceiveEndpointHandle> _handles;
        readonly object _mutateLock = new object();
        readonly PublishObservable _publishObservers;
        readonly ReceiveEndpointObservable _receiveEndpointObservers;
        readonly ReceiveObservable _receiveObservers;
        readonly SendObservable _sendObservers;

        public ReceiveEndpointCollection()
        {
            _endpoints = new Dictionary<string, IReceiveEndpointControl>(StringComparer.OrdinalIgnoreCase);
            _handles = new Dictionary<string, HostReceiveEndpointHandle>(StringComparer.OrdinalIgnoreCase);
            _receiveObservers = new ReceiveObservable();
            _receiveEndpointObservers = new ReceiveEndpointObservable();
            _consumeObservers = new ConsumeObservable();
            _publishObservers = new PublishObservable();
            _sendObservers = new SendObservable();
        }

        public void Add(string endpointName, IReceiveEndpointControl endpoint)
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));
            if (string.IsNullOrWhiteSpace(endpointName))
                throw new ArgumentException($"The {nameof(endpointName)} must not be null or empty", nameof(endpointName));

            lock (_mutateLock)
            {
                if (_endpoints.ContainsKey(endpointName))
                    throw new ConfigurationException($"A receive endpoint with the same key was already added: {endpointName}");

                _endpoints.Add(endpointName, endpoint);
            }
        }

        public HostReceiveEndpointHandle[] StartEndpoints()
        {
            KeyValuePair<string, IReceiveEndpointControl>[] startable;
            lock (_mutateLock)
            {
                startable = _endpoints.Where(x => !_handles.ContainsKey(x.Key)).ToArray();
            }

            return startable.Select(x => StartEndpoint(x.Key, x.Value)).ToArray();
        }

        public HostReceiveEndpointHandle Start(string endpointName)
        {
            if (string.IsNullOrWhiteSpace(endpointName))
                throw new ArgumentException($"The {nameof(endpointName)} must not be null or empty", nameof(endpointName));

            IReceiveEndpointControl endpoint;
            lock (_mutateLock)
            {
                if (!_endpoints.TryGetValue(endpointName, out endpoint))
                    throw new ConfigurationException($"A receive endpoint with the same key was already added: {endpointName}");

                if (_handles.ContainsKey(endpointName))
                    throw new ArgumentException($"The specified endpoint has already been started: {endpointName}", nameof(endpointName));
            }

            return StartEndpoint(endpointName, endpoint);
        }

        public void Probe(ProbeContext context)
        {
            foreach (KeyValuePair<string, IReceiveEndpointControl> receiveEndpoint in _endpoints)
            {
                var endpointScope = context.CreateScope("receiveEndpoint");
                endpointScope.Add("name", receiveEndpoint.Key);
                if (_handles.ContainsKey(receiveEndpoint.Key))
                    endpointScope.Add("started", true);
                receiveEndpoint.Value.Probe(endpointScope);
            }
        }

        public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _receiveObservers.Connect(observer);
        }

        public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _receiveEndpointObservers.Connect(observer);
        }

        public ConnectHandle ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer) where T : class
        {
            return new MultipleConnectHandle(_endpoints.Values.Select(x => x.ConnectConsumeMessageObserver(observer)));
        }

        public ConnectHandle ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _consumeObservers.Connect(observer);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _publishObservers.Connect(observer);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendObservers.Connect(observer);
        }

        HostReceiveEndpointHandle StartEndpoint(string endpointName, IReceiveEndpointControl endpoint)
        {
            try
            {
                var endpointReady = new ReceiveEndpointReadyObserver(endpoint);

                var consumeObserver = endpoint.ConnectConsumeObserver(_consumeObservers);
                var receiveObserver = endpoint.ConnectReceiveObserver(_receiveObservers);
                var receiveEndpointObserver = endpoint.ConnectReceiveEndpointObserver(_receiveEndpointObservers);
                var publishObserver = endpoint.ConnectPublishObserver(_publishObservers);
                var sendObserver = endpoint.ConnectSendObserver(_sendObservers);
                var endpointHandle = endpoint.Start();

                var handle = new Handle(endpointHandle, endpoint, endpointReady.Ready, () => Remove(endpointName),
                    receiveObserver, receiveEndpointObserver, consumeObserver, publishObserver, sendObserver);

                lock (_mutateLock)
                {
                    _handles.Add(endpointName, handle);
                }

                return handle;
            }
            catch
            {
                lock (_mutateLock)
                {
                    _endpoints.Remove(endpointName);
                }

                throw;
            }
        }

        void Remove(string endpointName)
        {
            if (string.IsNullOrWhiteSpace(endpointName))
                throw new ArgumentException($"The {nameof(endpointName)} must not be null or empty", nameof(endpointName));

            lock (_mutateLock)
            {
                _endpoints.Remove(endpointName);
                _handles.Remove(endpointName);
            }
        }


        class Handle :
            HostReceiveEndpointHandle
        {
            readonly ReceiveEndpointHandle _endpointHandle;
            readonly ConnectHandle[] _handles;
            readonly Action _onStopped;
            bool _stopped;

            public Handle(ReceiveEndpointHandle endpointHandle, IReceiveEndpoint receiveEndpoint, Task<ReceiveEndpointReady> ready, Action onStopped,
                params ConnectHandle[] handles)
            {
                _endpointHandle = endpointHandle;
                _onStopped = onStopped;
                _handles = handles;
                ReceiveEndpoint = receiveEndpoint;

                Ready = ready;
            }

            public IReceiveEndpoint ReceiveEndpoint { get; }
            public Task<ReceiveEndpointReady> Ready { get; }

            public async Task StopAsync(CancellationToken cancellationToken)
            {
                if (_stopped)
                    return;

                foreach (var handle in _handles)
                    handle.Disconnect();

                await _endpointHandle.Stop(cancellationToken).ConfigureAwait(false);

                _onStopped();

                _stopped = true;
            }
        }


        class ReceiveEndpointReadyObserver
        {
            readonly Observer _observer;

            public ReceiveEndpointReadyObserver(IReceiveEndpoint receiveEndpoint)
            {
                _observer = new Observer(receiveEndpoint);
            }

            public Task<ReceiveEndpointReady> Ready => _observer.Ready;


            class Observer :
                IReceiveEndpointObserver
            {
                readonly ConnectHandle _handle;
                readonly TaskCompletionSource<ReceiveEndpointReady> _ready;

                public Observer(IReceiveEndpoint endpoint)
                {
                    _ready = new TaskCompletionSource<ReceiveEndpointReady>();
                    _handle = endpoint.ConnectReceiveEndpointObserver(this);
                }

                public Task<ReceiveEndpointReady> Ready => _ready.Task;

                Task IReceiveEndpointObserver.Ready(ReceiveEndpointReady ready)
                {
                    _ready.TrySetResult(ready);

                    _handle.Disconnect();

                    return TaskUtil.Completed;
                }

                Task IReceiveEndpointObserver.Completed(ReceiveEndpointCompleted completed)
                {
                    return TaskUtil.Completed;
                }

                Task IReceiveEndpointObserver.Faulted(ReceiveEndpointFaulted faulted)
                {
                    _ready.TrySetExceptionWithBackgroundContinuations(faulted.Exception);

                    _handle.Disconnect();

                    return TaskUtil.Completed;
                }
            }
        }
    }
}