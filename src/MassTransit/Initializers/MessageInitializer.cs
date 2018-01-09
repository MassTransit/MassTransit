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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    /// Initializes a message using the input, which can include message properties, headers, etc.
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    /// <typeparam name="TInput">The input type</typeparam>
    public class MessageInitializer<TMessage, TInput> :
        IMessageInitializer<TMessage, TInput>
        where TMessage : class
        where TInput : class
    {
        readonly IMessageFactory<TMessage> _factory;
        readonly IMessageHeaderInitializer<TMessage, TInput>[] _headerInitializers;
        readonly IMessagePropertyInitializer<TMessage, TInput>[] _initializers;

        public MessageInitializer(IMessageFactory<TMessage> factory, IEnumerable<IMessagePropertyInitializer<TMessage, TInput>> initializers,
            IEnumerable<IMessageHeaderInitializer<TMessage, TInput>> headerInitializers)
        {
            _factory = factory;
            _initializers = initializers.ToArray();
            _headerInitializers = headerInitializers.ToArray();
        }

        public async Task<TMessage> Initialize(TInput input, CancellationToken cancellationToken)
        {
            var context = await InitializeMessage(input, cancellationToken).ConfigureAwait(false);

            return context.Message;
        }

        public async Task Send(ISendEndpoint endpoint, TInput input, IPipe<SendContext<TMessage>> pipe, CancellationToken cancellationToken)
        {
            InitializeMessageContext<TMessage, TInput> messageContext = await InitializeMessage(input, cancellationToken).ConfigureAwait(false);

            await endpoint.Send(messageContext.Message,
                _headerInitializers.Length > 0 ? new InitializerSendContextPipe(_headerInitializers, messageContext, pipe) : pipe, cancellationToken);
        }

        public async Task Send(ISendEndpoint endpoint, TInput input, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            InitializeMessageContext<TMessage, TInput> messageContext = await InitializeMessage(input, cancellationToken).ConfigureAwait(false);

            if (_headerInitializers.Length > 0)
                await endpoint.Send(messageContext.Message, new InitializerSendContextPipe(_headerInitializers, messageContext, pipe), cancellationToken)
                    .ConfigureAwait(false);
            else
                await endpoint.Send(messageContext.Message, pipe, cancellationToken).ConfigureAwait(false);
        }

        public async Task Publish(IPublishEndpoint endpoint, TInput input, IPipe<PublishContext<TMessage>> pipe, CancellationToken cancellationToken)
        {
            InitializeMessageContext<TMessage, TInput> messageContext = await InitializeMessage(input, cancellationToken).ConfigureAwait(false);

            await endpoint.Publish(messageContext.Message,
                _headerInitializers.Length > 0 ? new InitializerPublishContextPipe(_headerInitializers, messageContext, pipe) : pipe, cancellationToken);
        }

        public async Task Publish(IPublishEndpoint endpoint, TInput input, IPipe<PublishContext> pipe, CancellationToken cancellationToken)
        {
            InitializeMessageContext<TMessage, TInput> messageContext = await InitializeMessage(input, cancellationToken).ConfigureAwait(false);

            if (_headerInitializers.Length > 0)
                await endpoint.Publish(messageContext.Message, new InitializerPublishContextPipe(_headerInitializers, messageContext, pipe), cancellationToken)
                    .ConfigureAwait(false);
            else
                await endpoint.Publish(messageContext.Message, pipe, cancellationToken).ConfigureAwait(false);
        }

        async Task<InitializeMessageContext<TMessage, TInput>> InitializeMessage(TInput input, CancellationToken cancellationToken)
        {
            var context = new BaseInitializeMessageContext(typeof(TMessage), cancellationToken);

            InitializeMessageContext<TMessage> messageContext = _factory.Create(context);

            InitializeMessageContext<TMessage, TInput> inputContext = messageContext.CreateInputContext<TInput>(input);

            await Task.WhenAll(_initializers.Select(x => x.Apply(inputContext))).ConfigureAwait(false);

            return inputContext;
        }


        class InitializerSendContextPipe :
            IPipe<SendContext<TMessage>>
        {
            readonly InitializeMessageContext<TMessage, TInput> _context;
            readonly IMessageHeaderInitializer<TMessage, TInput>[] _initializers;
            readonly IPipe<SendContext<TMessage>> _pipe;
            readonly IPipe<SendContext> _sendPipe;

            public InitializerSendContextPipe(IMessageHeaderInitializer<TMessage, TInput>[] initializers, InitializeMessageContext<TMessage, TInput> context,
                IPipe<SendContext<TMessage>> pipe)
            {
                _initializers = initializers;
                _pipe = pipe;
                _context = context;
            }

            public InitializerSendContextPipe(IMessageHeaderInitializer<TMessage, TInput>[] initializers, InitializeMessageContext<TMessage, TInput> context,
                IPipe<SendContext> pipe)
            {
                _initializers = initializers;
                _sendPipe = pipe;
                _context = context;
            }

            void IProbeSite.Probe(ProbeContext context)
            {
                _pipe?.Probe(context);
                _sendPipe?.Probe(context);
            }

            public async Task Send(SendContext<TMessage> context)
            {
                await Task.WhenAll(_initializers.Select(x => x.Apply(_context, context))).ConfigureAwait(false);

                if (_pipe != null)
                    await _pipe.Send(context).ConfigureAwait(false);

                if (_sendPipe != null)
                    await _sendPipe.Send(context).ConfigureAwait(false);
            }
        }


        class InitializerPublishContextPipe :
            IPipe<PublishContext<TMessage>>
        {
            readonly InitializeMessageContext<TMessage, TInput> _context;
            readonly IMessageHeaderInitializer<TMessage, TInput>[] _initializers;
            readonly IPipe<PublishContext<TMessage>> _pipe;
            readonly IPipe<PublishContext> _sendPipe;

            public InitializerPublishContextPipe(IMessageHeaderInitializer<TMessage, TInput>[] initializers, InitializeMessageContext<TMessage, TInput> context,
                IPipe<PublishContext<TMessage>> pipe)
            {
                _initializers = initializers;
                _pipe = pipe;
                _context = context;
            }

            public InitializerPublishContextPipe(IMessageHeaderInitializer<TMessage, TInput>[] initializers, InitializeMessageContext<TMessage, TInput> context,
                IPipe<PublishContext> pipe)
            {
                _initializers = initializers;
                _sendPipe = pipe;
                _context = context;
            }

            void IProbeSite.Probe(ProbeContext context)
            {
                _pipe?.Probe(context);
                _sendPipe?.Probe(context);
            }

            public async Task Send(PublishContext<TMessage> context)
            {
                await Task.WhenAll(_initializers.Select(x => x.Apply(_context, context))).ConfigureAwait(false);

                if (_pipe != null)
                    await _pipe.Send(context).ConfigureAwait(false);

                if (_sendPipe != null)
                    await _sendPipe.Send(context).ConfigureAwait(false);
            }
        }
    }
}