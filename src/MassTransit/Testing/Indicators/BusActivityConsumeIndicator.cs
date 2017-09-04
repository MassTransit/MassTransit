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


    public class BusActivityConsumeIndicator : BaseBusActivityIndicatorConnectable,
        ISignalResource,
        IConsumeObserver
    {
        readonly ISignalResource _signalResource;
        int _messagesInFlight;

        public BusActivityConsumeIndicator(ISignalResource signalResource)
        {
            _signalResource = signalResource;
        }

        public BusActivityConsumeIndicator()
            :
                this(null)
        {
        }

        public override bool IsMet => Interlocked.CompareExchange(ref _messagesInFlight, int.MinValue, int.MinValue) == 0;

        Task IConsumeObserver.PreConsume<T>(ConsumeContext<T> context)
        {
            Interlocked.Increment(ref _messagesInFlight);
            return TaskUtil.Completed;
        }

        Task IConsumeObserver.PostConsume<T>(ConsumeContext<T> context)
        {
            if (Interlocked.Decrement(ref _messagesInFlight) == 0)
                Signal();
            return TaskUtil.Completed;
        }

        Task IConsumeObserver.ConsumeFault<T>(ConsumeContext<T> context, Exception exception)
        {
            if (Interlocked.Decrement(ref _messagesInFlight) == 0)
                Signal();
            return TaskUtil.Completed;
        }

        public void Signal()
        {
            _signalResource?.Signal();
            ConditionUpdated();
        }
    }
}