// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Pipeline.Pipes
{
    using System;
    using System.Threading.Tasks;
    using Context.Converters;
    using GreenPipes;


    public class ConsumeObserverAdapter :
        IFilterObserver
    {
        readonly IConsumeObserver _observer;

        public ConsumeObserverAdapter(IConsumeObserver observer)
        {
            _observer = observer;
        }

        Task IFilterObserver.PreSend<T>(T context)
        {
            return ConsumeObserverConverterCache.PreConsume(typeof(T), _observer, context);
        }

        Task IFilterObserver.PostSend<T>(T context)
        {
            return ConsumeObserverConverterCache.PostConsume(typeof(T), _observer, context);
        }

        Task IFilterObserver.SendFault<T>(T context, Exception exception)
        {
            return ConsumeObserverConverterCache.ConsumeFault(typeof(T), _observer, context, exception);
        }
    }


    public class ConsumeObserverAdapter<T> :
        IFilterObserver<ConsumeContext<T>>
        where T : class
    {
        readonly IConsumeMessageObserver<T> _observer;

        public ConsumeObserverAdapter(IConsumeMessageObserver<T> observer)
        {
            _observer = observer;
        }

        Task IFilterObserver<ConsumeContext<T>>.PreSend(ConsumeContext<T> context)
        {
            return _observer.PreConsume(context);
        }

        Task IFilterObserver<ConsumeContext<T>>.PostSend(ConsumeContext<T> context)
        {
            return _observer.PostConsume(context);
        }

        Task IFilterObserver<ConsumeContext<T>>.SendFault(ConsumeContext<T> context, Exception exception)
        {
            return _observer.ConsumeFault(context, exception);
        }
    }
}