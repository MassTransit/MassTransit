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
namespace MassTransit.Testing.Indicators
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Util;


    /// <summary>
    /// An activity indicator for publish endpoints. Utilizes a timer that restarts on publish activity.
    /// </summary>
    public class BusActivityPublishIndicator : BaseBusActivityIndicatorConnectable,
        ISignalResource,
        IPublishObserver
    {
        readonly RollingTimer _receiveIdleTimer;
        readonly ISignalResource _signalResource;
        int _activityStarted;

        public BusActivityPublishIndicator(ISignalResource signalResource, TimeSpan receiveIdleTimeout)
        {
            _signalResource = signalResource;
            _receiveIdleTimer = new RollingTimer(SignalInactivity, receiveIdleTimeout);
        }

        public BusActivityPublishIndicator(ISignalResource signalResource)
            :
                this(signalResource, TimeSpan.FromSeconds(5))
        {
        }

        public BusActivityPublishIndicator(TimeSpan receiveIdleTimeout)
            :
                this(null, receiveIdleTimeout)
        {
        }

        public BusActivityPublishIndicator()
            :
                this(null)
        {
        }

        public override bool IsMet => _receiveIdleTimer.Triggered ||
            Interlocked.CompareExchange(ref _activityStarted, int.MinValue, int.MinValue) == 0;

        Task IPublishObserver.PrePublish<T>(PublishContext<T> context)
        {
            Interlocked.CompareExchange(ref _activityStarted, 1, 0);
            _receiveIdleTimer.Restart();
            return TaskUtil.Completed;
        }

        Task IPublishObserver.PostPublish<T>(PublishContext<T> context)
        {
            _receiveIdleTimer.Restart();
            return TaskUtil.Completed;
        }

        Task IPublishObserver.PublishFault<T>(PublishContext<T> context, Exception exception)
        {
            _receiveIdleTimer.Restart();
            return TaskUtil.Completed;
        }

        public void Signal()
        {
            SignalInactivity(null);
        }

        void SignalInactivity(object state)
        {
            _signalResource?.Signal();
            ConditionUpdated();
            Interlocked.CompareExchange(ref _activityStarted, 0, 1);
            _receiveIdleTimer.Stop();
        }
    }
}