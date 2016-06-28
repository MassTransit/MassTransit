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
namespace Automatonymous
{
    using System;
    using Activities;
    using Binders;
    using MassTransit;


    public static class SendExtensions
    {
        public static EventActivityBinder<TInstance> Send<TInstance, TMessage>(this EventActivityBinder<TInstance> source, Uri destinationAddress,
            TMessage message)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TMessage>(destinationAddress, x => message));
        }

        public static EventActivityBinder<TInstance> Send<TInstance, TMessage>(this EventActivityBinder<TInstance> source, Uri destinationAddress,
            TMessage message, Action<SendContext<TMessage>> contextCallback)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TMessage>(destinationAddress, x => message, contextCallback));
        }

        public static EventActivityBinder<TInstance> Send<TInstance, TMessage>(
            this EventActivityBinder<TInstance> source, Uri destinationAddress,
            EventMessageFactory<TInstance, TMessage> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TMessage>(destinationAddress, messageFactory));
        }

        public static EventActivityBinder<TInstance> Send<TInstance, TMessage>(
            this EventActivityBinder<TInstance> source, Uri destinationAddress,
            EventMessageFactory<TInstance, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TMessage>(destinationAddress, messageFactory, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> Send<TInstance, TData, TMessage>(
            this EventActivityBinder<TInstance, TData> source, Uri destinationAddress, TMessage message)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TData, TMessage>(destinationAddress, x => message));
        }

        public static EventActivityBinder<TInstance, TData> Send<TInstance, TData, TMessage>(
            this EventActivityBinder<TInstance, TData> source, Uri destinationAddress, TMessage message,
            Action<SendContext<TMessage>> contextCallback)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TData, TMessage>(destinationAddress, x => message, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> Send<TInstance, TData, TMessage>(
            this EventActivityBinder<TInstance, TData> source, Uri destinationAddress, EventMessageFactory<TInstance, TData, TMessage> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TData, TMessage>(destinationAddress, messageFactory));
        }

        public static EventActivityBinder<TInstance, TData> Send<TInstance, TData, TMessage>(
            this EventActivityBinder<TInstance, TData> source, Uri destinationAddress, EventMessageFactory<TInstance, TData, TMessage> messageFactory,
            Action<SendContext<TMessage>> contextCallback)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TData, TMessage>(destinationAddress, messageFactory, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Send<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Uri destinationAddress, TMessage message)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new SendActivity<TInstance, TData, TMessage>(destinationAddress, x => message));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Send<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Uri destinationAddress, TMessage message,
            Action<SendContext<TMessage>> contextCallback)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new SendActivity<TInstance, TData, TMessage>(destinationAddress, x => message, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Send<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Uri destinationAddress,
            EventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TInstance, TData, TException, TMessage>(destinationAddress, messageFactory));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Send<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Uri destinationAddress,
            EventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory,
            Action<SendContext<TMessage>> contextCallback)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TInstance, TData, TException, TMessage>(destinationAddress, messageFactory, contextCallback));
        }
    }
}