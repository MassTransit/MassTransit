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
namespace MassTransit.Initializers
{
    using System;
    using System.Threading;
    using GreenPipes;


    public class DynamicInitializeMessageContext<TMessage> :
        InitializeMessageContext<TMessage>
        where TMessage : class
    {
        readonly InitializeMessageContext _context;

        public DynamicInitializeMessageContext(InitializeMessageContext context, TMessage message)
        {
            _context = context;
            Message = message;
        }

        public TMessage Message { get; }

        public InitializeMessageContext<TMessage, T> CreateInputContext<T>(T input)
            where T : class
        {
            return new DynamicInitializeMessageContext<TMessage, T>(this, Message, input);
        }

        bool PipeContext.HasPayloadType(Type payloadType)
        {
            return _context.HasPayloadType(payloadType);
        }

        bool PipeContext.TryGetPayload<TPayload>(out TPayload payload)
        {
            return _context.TryGetPayload(out payload);
        }

        TPayload PipeContext.GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
        {
            return _context.GetOrAddPayload(payloadFactory);
        }

        T PipeContext.AddOrUpdatePayload<T>(PayloadFactory<T> addFactory, UpdatePayloadFactory<T> updateFactory)
        {
            return _context.AddOrUpdatePayload(addFactory, updateFactory);
        }

        CancellationToken PipeContext.CancellationToken => _context.CancellationToken;

        Type InitializeMessageContext.MessageType => _context.MessageType;
    }


    public class DynamicInitializeMessageContext<TMessage, TInput> :
        InitializeMessageContext<TMessage, TInput>
        where TMessage : class
        where TInput : class
    {
        readonly InitializeMessageContext _context;

        public DynamicInitializeMessageContext(InitializeMessageContext context, TMessage message, TInput input)
        {
            _context = context;
            Message = message;
            Input = input;
        }

        public TMessage Message { get; }
        public TInput Input { get; }

        public InitializeMessageContext<TMessage, T> CreateInputContext<T>(T input)
            where T : class
        {
            return new DynamicInitializeMessageContext<TMessage, T>(this, Message, input);
        }

        bool PipeContext.HasPayloadType(Type payloadType)
        {
            return _context.HasPayloadType(payloadType);
        }

        bool PipeContext.TryGetPayload<TPayload>(out TPayload payload)
        {
            return _context.TryGetPayload(out payload);
        }

        TPayload PipeContext.GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
        {
            return _context.GetOrAddPayload(payloadFactory);
        }

        T PipeContext.AddOrUpdatePayload<T>(PayloadFactory<T> addFactory, UpdatePayloadFactory<T> updateFactory)
        {
            return _context.AddOrUpdatePayload(addFactory, updateFactory);
        }

        CancellationToken PipeContext.CancellationToken => _context.CancellationToken;

        Type InitializeMessageContext.MessageType => _context.MessageType;
    }
}