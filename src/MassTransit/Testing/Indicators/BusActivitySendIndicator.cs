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
    /// An activity indicator for send endpoints. Utilizes a timer that restarts on send activity.
    /// </summary>
    public class BusActivitySendIndicator : BaseBusActivityIndicatorConnectable,
        ISignalResource,
        ISendObserver
    {
        readonly RollingTimer _receiveIdleTimer;
        readonly ISignalResource _signalResource;
        int _activityStarted;

        public BusActivitySendIndicator(ISignalResource signalResource, TimeSpan receiveIdleTimeout)
        {
            _signalResource = signalResource;
            _receiveIdleTimer = new RollingTimer(SignalInactivity, receiveIdleTimeout);
        }

        public BusActivitySendIndicator(ISignalResource signalResource)
            :
                this(signalResource, TimeSpan.FromSeconds(5))
        {
        }

        public BusActivitySendIndicator(TimeSpan receiveIdleTimeout)
            :
                this(null, receiveIdleTimeout)
        {
        }

        public BusActivitySendIndicator()
            :
                this(null)
        {
        }

        public override bool IsMet => _receiveIdleTimer.Triggered ||
            Interlocked.CompareExchange(ref _activityStarted, int.MinValue, int.MinValue) == 0;

        Task ISendObserver.PreSend<T>(SendContext<T> context)
        {
            Interlocked.CompareExchange(ref _activityStarted, 1, 0);
            _receiveIdleTimer.Restart();
            return TaskUtil.Completed;
        }

        Task ISendObserver.PostSend<T>(SendContext<T> context)
        {
            _receiveIdleTimer.Restart();
            return TaskUtil.Completed;
        }

        Task ISendObserver.SendFault<T>(SendContext<T> context, Exception exception)
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