namespace MassTransit.SignalR.Tests.OfficialFramework
{
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using System.IO.Pipelines;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Connections;
    using Microsoft.AspNetCore.Connections.Features;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.AspNetCore.SignalR.Protocol;
    using Util;


    class TestClient : ITransferFormatFeature,
        IConnectionHeartbeatFeature,
        IDisposable
    {
        static int _id;
        readonly CancellationTokenSource _cts;
        readonly object _heartbeatLock = new object();
        readonly IInvocationBinder _invocationBinder;
        readonly IHubProtocol _protocol;
        List<(Action<object> handler, object state)> _heartbeatHandlers;

        public TestClient(IHubProtocol protocol = null, IInvocationBinder invocationBinder = null, string userIdentifier = null)
        {
            var options = new PipeOptions(readerScheduler: PipeScheduler.Inline, writerScheduler: PipeScheduler.Inline, useSynchronizationContext: false);
            var pair = DuplexPipe.CreateConnectionPair(options, options);
            Connection = new DefaultConnectionContext(Guid.NewGuid().ToString(), pair.Transport, pair.Application);

            // Add features SignalR needs for testing
            Connection.Features.Set<ITransferFormatFeature>(this);
            Connection.Features.Set<IConnectionHeartbeatFeature>(this);

            var claimValue = Interlocked.Increment(ref _id).ToString();
            var claims = new List<Claim> {new Claim(ClaimTypes.Name, claimValue)};
            if (userIdentifier != null)
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userIdentifier));

            Connection.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
            Connection.Items["ConnectedTask"] = TaskUtil.GetTask<bool>();

            _protocol = protocol ?? new JsonHubProtocol();
            _invocationBinder = invocationBinder ?? new DefaultInvocationBinder();

            _cts = new CancellationTokenSource();
        }

        public DefaultConnectionContext Connection { get; }
        public Task Connected => ((TaskCompletionSource<bool>)Connection.Items["ConnectedTask"]).Task;
        public HandshakeResponseMessage HandshakeResponseMessage { get; private set; }

        public void OnHeartbeat(Action<object> action, object state)
        {
            lock (_heartbeatLock)
            {
                if (_heartbeatHandlers == null)
                    _heartbeatHandlers = new List<(Action<object> handler, object state)>();

                _heartbeatHandlers.Add((action, state));
            }
        }

        public void Dispose()
        {
            _cts.Cancel();

            Connection.Application.Output.Complete();
        }

        public TransferFormat SupportedFormats { get; set; } = TransferFormat.Text | TransferFormat.Binary;

        public TransferFormat ActiveFormat { get; set; }

        public async Task<Task> ConnectAsync(ConnectionHandler handler,
            bool sendHandshakeRequestMessage = true,
            bool expectedHandshakeResponseMessage = true)
        {
            if (sendHandshakeRequestMessage)
            {
                var memoryBufferWriter = MemoryBufferWriter.Get();
                try
                {
                    HandshakeProtocol.WriteRequestMessage(new HandshakeRequestMessage(_protocol.Name, _protocol.Version), memoryBufferWriter);
                    await Connection.Application.Output.WriteAsync(memoryBufferWriter.ToArray());
                }
                finally
                {
                    MemoryBufferWriter.Return(memoryBufferWriter);
                }
            }

            var connection = handler.OnConnectedAsync(Connection);

            if (expectedHandshakeResponseMessage)
            {
                // note that the handshake response might not immediately be readable
                // e.g. server is waiting for request, times out after configured duration,
                // and sends response with timeout error
                HandshakeResponseMessage = (HandshakeResponseMessage)await ReadAsync(true).OrTimeout();
            }

            return connection;
        }

        public async Task<IList<HubMessage>> StreamAsync(string methodName, params object[] args)
        {
            var invocationId = await SendStreamInvocationAsync(methodName, args);

            var messages = new List<HubMessage>();
            while (true)
            {
                var message = await ReadAsync();

                if (message == null)
                    throw new InvalidOperationException("Connection aborted!");

                if (message is HubInvocationMessage hubInvocationMessage && !string.Equals(hubInvocationMessage.InvocationId, invocationId))
                    throw new NotSupportedException("TestClient does not support multiple outgoing invocations!");

                switch (message)
                {
                    case StreamItemMessage _:
                        messages.Add(message);
                        break;
                    case CompletionMessage _:
                        messages.Add(message);
                        return messages;
                    default:
                        throw new NotSupportedException("TestClient does not support receiving invocations!");
                }
            }
        }

        public async Task<CompletionMessage> InvokeAsync(string methodName, params object[] args)
        {
            var invocationId = await SendInvocationAsync(methodName, false, args);

            while (true)
            {
                var message = await ReadAsync();

                if (message == null)
                    throw new InvalidOperationException("Connection aborted!");

                if (message is HubInvocationMessage hubInvocationMessage && !string.Equals(hubInvocationMessage.InvocationId, invocationId))
                    throw new NotSupportedException("TestClient does not support multiple outgoing invocations!");

                switch (message)
                {
                    case StreamItemMessage result:
                        throw new NotSupportedException("Use 'StreamAsync' to call a streaming method");
                    case CompletionMessage completion:
                        return completion;
                    case PingMessage _:
                        // Pings are ignored
                        break;
                    default:
                        throw new NotSupportedException("TestClient does not support receiving invocations!");
                }
            }
        }

        public Task<string> SendInvocationAsync(string methodName, params object[] args)
        {
            return SendInvocationAsync(methodName, false, args);
        }

        public Task<string> SendInvocationAsync(string methodName, bool nonBlocking, params object[] args)
        {
            var invocationId = nonBlocking ? null : GetInvocationId();
            return SendHubMessageAsync(new InvocationMessage(invocationId, methodName, args));
        }

        public Task<string> SendStreamInvocationAsync(string methodName, params object[] args)
        {
            var invocationId = GetInvocationId();
            return SendHubMessageAsync(new StreamInvocationMessage(invocationId, methodName, args));
        }

        public async Task<string> SendHubMessageAsync(HubMessage message)
        {
            ReadOnlyMemory<byte> payload = _protocol.GetMessageBytes(message);

            await Connection.Application.Output.WriteAsync(payload);
            return message is HubInvocationMessage hubMessage ? hubMessage.InvocationId : null;
        }

        public async Task<HubMessage> ReadAsync(bool isHandshake = false)
        {
            while (true)
            {
                var message = TryRead(isHandshake);

                if (message == null)
                {
                    var result = await Connection.Application.Input.ReadAsync();
                    ReadOnlySequence<byte> buffer = result.Buffer;

                    try
                    {
                        if (!buffer.IsEmpty)
                            continue;

                        if (result.IsCompleted)
                            return null;
                    }
                    finally
                    {
                        Connection.Application.Input.AdvanceTo(buffer.Start);
                    }
                }
                else
                    return message;
            }
        }

        public HubMessage TryRead(bool isHandshake = false)
        {
            if (!Connection.Application.Input.TryRead(out var result))
                return null;

            ReadOnlySequence<byte> buffer = result.Buffer;

            try
            {
                if (!isHandshake)
                {
                    if (_protocol.TryParseMessage(ref buffer, _invocationBinder, out var message))
                        return message;
                }
                else
                {
                    // read first message out of the incoming data
                    if (HandshakeProtocol.TryParseResponseMessage(ref buffer, out var responseMessage))
                        return responseMessage;
                }
            }
            finally
            {
                Connection.Application.Input.AdvanceTo(buffer.Start);
            }

            return null;
        }

        static string GetInvocationId()
        {
            return Guid.NewGuid().ToString("N");
        }

        public void TickHeartbeat()
        {
            lock (_heartbeatLock)
            {
                if (_heartbeatHandlers == null)
                    return;

                foreach ((Action<object> handler, var state) in _heartbeatHandlers)
                    handler(state);
            }
        }


        class DefaultInvocationBinder : IInvocationBinder
        {
            public IReadOnlyList<Type> GetParameterTypes(string methodName)
            {
                // TODO: Possibly support actual client methods
                return new[] {typeof(object)};
            }

            public Type GetStreamItemType(string streamId)
            {
                return typeof(object);
            }

            public Type GetReturnType(string invocationId)
            {
                return typeof(object);
            }
        }
    }
}
