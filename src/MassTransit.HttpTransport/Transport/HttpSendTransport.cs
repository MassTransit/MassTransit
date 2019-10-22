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
namespace MassTransit.HttpTransport.Transport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using Clients;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using Context;
    using MassTransit.Pipeline.Observables;
    using Metadata;
    using Transports;


    public class HttpSendTransport :
        Supervisor,
        ISendTransport
    {
        readonly IClientContextSupervisor _clientContextSupervisor;
        readonly SendObservable _observers;
        readonly HttpSendSettings _sendSettings;
        readonly ReceiveEndpointContext _topology;

        public HttpSendTransport(IClientContextSupervisor clientContextSupervisor, HttpSendSettings sendSettings, ReceiveEndpointContext topology)
        {
            _clientContextSupervisor = clientContextSupervisor;
            _sendSettings = sendSettings;
            _topology = topology;

            _observers = new SendObservable();

            Add(clientContextSupervisor);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _observers.Connect(observer);
        }

        public async Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            IPipe<ClientContext> clientPipe = Pipe.New<ClientContext>(p =>
            {
                p.UseExecuteAsync(async clientContext =>
                {
                    var context = new HttpSendContextImpl<T>(message, cancellationToken);

                    try
                    {
                        await pipe.Send(context).ConfigureAwait(false);

                        using (var request = new HttpRequestMessage(_sendSettings.Method, context.DestinationAddress))
                        using (var payload = new ByteArrayContent(context.Body))
                        {
                            request.Headers.UserAgent.Add(new ProductInfoHeaderValue("MassTransit", HostMetadataCache.Host.MassTransitVersion));

                            if (context.ResponseAddress != null)
                                request.Headers.Referrer = context.ResponseAddress;

                            payload.Headers.ContentType = new MediaTypeHeaderValue(context.ContentType.MediaType);

                            foreach (KeyValuePair<string, object> header in
                                context.Headers.GetAll().Where(h => h.Value != null && (h.Value is string || h.Value.GetType().IsValueType)))
                                request.Headers.Add(header.Key, header.Value.ToString());

                            if (context.MessageId.HasValue)
                                request.Headers.Add(Clients.HttpHeaders.MessageId, context.MessageId.Value.ToString());

                            if (context.CorrelationId.HasValue)
                                request.Headers.Add(Clients.HttpHeaders.CorrelationId, context.CorrelationId.Value.ToString());

                            if (context.InitiatorId.HasValue)
                                request.Headers.Add(Clients.HttpHeaders.InitiatorId, context.InitiatorId.Value.ToString());

                            if (context.ConversationId.HasValue)
                                request.Headers.Add(Clients.HttpHeaders.ConversationId, context.ConversationId.Value.ToString());

                            if (context.RequestId.HasValue)
                                request.Headers.Add(Clients.HttpHeaders.RequestId, context.RequestId.Value.ToString());

                            //TODO: TTL?

                            request.Content = payload;

                            await _observers.PreSend(context).ConfigureAwait(false);

                            using (var response = await clientContext.SendAsync(request, cancellationToken).ConfigureAwait(false))
                            {
                                response.EnsureSuccessStatusCode();

                                var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

                                if (responseStream.Length > 0)
                                {
                                    var receiveContext = new HttpClientReceiveContext(response, responseStream, false, _topology);

                                    await clientContext.ReceiveResponse(receiveContext).ConfigureAwait(false);
                                }
                            }

                            await _observers.PostSend(context).ConfigureAwait(false);
                        }
                    }
                    catch (Exception ex)
                    {
                        await _observers.SendFault(context, ex).ConfigureAwait(false);

                        throw;
                    }
                });
            });

            await _clientContextSupervisor.Send(clientPipe, cancellationToken).ConfigureAwait(false);
        }
    }
}
