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
namespace MassTransit.Context
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.Mime;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Payloads;
    using Serialization;
    using Topology;
    using Transports;
    using Util;


    public abstract class BaseReceiveContext :
        BasePipeContext,
        ReceiveContext,
        IDisposable
    {
        static readonly ContentType DefaultContentType = JsonMessageSerializer.JsonContentType;
        readonly CancellationTokenSource _cancellationTokenSource;
        readonly Lazy<ContentType> _contentType;
        readonly Lazy<Headers> _headers;
        readonly List<Task> _pendingTasks;
        readonly IReceiveObserver _receiveObserver;
        readonly Stopwatch _receiveTimer;

        protected BaseReceiveContext(Uri inputAddress, bool redelivered, IReceiveObserver receiveObserver, IReceiveEndpointTopology topology)
            : this(inputAddress, redelivered, receiveObserver, new CancellationTokenSource(), topology)
        {
        }

        protected BaseReceiveContext(Uri inputAddress, bool redelivered, IReceiveObserver receiveObserver, CancellationTokenSource source, IReceiveEndpointTopology topology)
            : base(new PayloadCache(), source.Token)
        {
            _receiveTimer = Stopwatch.StartNew();

            _cancellationTokenSource = source;
            Topology = topology;
            InputAddress = inputAddress;
            Redelivered = redelivered;
            _receiveObserver = receiveObserver;

            _headers = new Lazy<Headers>(() => new JsonHeaders(ObjectTypeDeserializer.Instance, HeaderProvider));

            _contentType = new Lazy<ContentType>(GetContentType);

            _pendingTasks = new List<Task>(4);
        }

        protected abstract IHeaderProvider HeaderProvider { get; }
        public bool IsDelivered { get; private set; }
        public bool IsFaulted { get; private set; }

        public ISendEndpointProvider SendEndpointProvider => Topology.SendEndpointProvider;
        public IPublishEndpointProvider PublishEndpointProvider => Topology.PublishEndpointProvider;
        public ISendTransportProvider SendTransportProvider => Topology.SendTransportProvider;
        public IReceiveEndpointTopology Topology { get; }

        public Task CompleteTask
        {
            get
            {
                Task[] tasks;
                lock (_pendingTasks)
                {
                    tasks = _pendingTasks
                        .Where(x => x.Status != TaskStatus.RanToCompletion)
                        .ToArray();
                }


                var completeTask = Task.WhenAll(tasks);
                completeTask.ContinueWith(result =>
                {
                    lock (_pendingTasks)
                    {
                        for (var i = 0; i < _pendingTasks.Count;)
                        {
                            if (_pendingTasks[i].Status == TaskStatus.RanToCompletion)
                            {
                                _pendingTasks.RemoveAt(i);
                            }
                            else
                            {
                                i++;
                            }
                        }
                    }
                }, TaskContinuationOptions.OnlyOnRanToCompletion);


                return completeTask;
            }
        }

        public void AddPendingTask(Task task)
        {
            lock (_pendingTasks)
                _pendingTasks.Add(task);
        }

        public bool Redelivered { get; }
        public Headers TransportHeaders => _headers.Value;

        public virtual Task NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType) where T : class
        {
            IsDelivered = true;

            context.LogConsumed(duration, consumerType);

            return _receiveObserver.PostConsume(context, duration, consumerType);
        }

        public virtual Task NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception) where T : class
        {
            IsFaulted = true;

            context.LogFaulted(duration, consumerType, exception);

            return _receiveObserver.ConsumeFault(context, duration, consumerType, exception);
        }

        public virtual Task NotifyFaulted(Exception exception)
        {
            IsFaulted = true;

            return _receiveObserver.ReceiveFault(this, exception);
        }

        public virtual Stream GetBody()
        {
            return GetBodyStream();
        }

        public TimeSpan ElapsedTime => _receiveTimer.Elapsed;
        public Uri InputAddress { get; }
        public ContentType ContentType => _contentType.Value;
        protected abstract Stream GetBodyStream();

        protected virtual ContentType GetContentType()
        {
            object contentTypeHeader;
            if (_headers.Value.TryGetHeader("Content-Type", out contentTypeHeader))
            {
                var contentType = contentTypeHeader as ContentType;
                if (contentType != null)
                    return contentType;

                var contentTypeString = contentTypeHeader as string;
                if (contentTypeString != null)
                    return new ContentType(contentTypeString);
            }

            return DefaultContentType;
        }

        public void Cancel()
        {
            _cancellationTokenSource.Cancel();
        }

        public void Dispose()
        {
            _cancellationTokenSource.Dispose();
        }
    }
}