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
namespace MassTransit.Pipeline.Observables
{
    using System;
    using System.Threading.Tasks;


    public class SendMessageObserverAdapter<T> :
        ISendMessageObserver<T>
        where T : class
    {
        readonly ISendObserver _observer;

        public SendMessageObserverAdapter(ISendObserver observer)
        {
            _observer = observer;
        }

        public Task PreSend(SendContext<T> context)
        {
            return _observer.PreSend(context);
        }

        public Task PostSend(SendContext<T> context)
        {
            return _observer.PostSend(context);
        }

        public Task SendFault(SendContext<T> context, Exception exception)
        {
            return _observer.SendFault(context, exception);
        }
    }
}