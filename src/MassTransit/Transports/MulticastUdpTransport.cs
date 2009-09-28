// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Transports
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using Exceptions;

    [DebuggerDisplay("{Address}")]
    public class MulticastUdpTransport :
        ITransport
    {
        private bool _disposed;
        private IPAddress _groupAddress;
        private UdpClient _receiveClient;
        private UdpClient _sendClient;
        private IPEndPoint _sendIpEndPoint;

        public MulticastUdpTransport(IEndpointAddress address)
        {
            Address = address;

            Initialize();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IEndpointAddress Address { get; private set; }

        public void Send(Action<Stream> sender)
        {
            if (_disposed) throw NewDisposedException();

            using (var bodyStream = new MemoryStream())
            {
                sender(bodyStream);

                try
                {
                    byte[] data = bodyStream.ToArray();
                    _sendClient.Send(data, data.Length, _sendIpEndPoint);
                }
                catch (Exception ex)
                {
                    throw new TransportException(Address.Uri, "Unable to send to transport: " + _sendIpEndPoint, ex);
                }
            }
        }

        public void Receive(Func<Stream, Action<Stream>> receiver)
        {
            if (_disposed) throw NewDisposedException();

            Receive(receiver, TimeSpan.Zero);
        }

        public void Receive(Func<Stream, Action<Stream>> receiver, TimeSpan timeout)
        {
            if (_disposed) throw NewDisposedException();

            var endPoint = new IPEndPoint(IPAddress.Any, Address.Uri.Port);

            byte[] data = _receiveClient.Receive(ref endPoint);
            if (data == null || data.Length <= 0)
                return;

            using (var bodyStream = new MemoryStream(data))
            {
                Action<Stream> receive = receiver(bodyStream);
                if (receive == null)
                {
                    // SKIPPED
                    return;
                }

                receive(bodyStream);
            }
        }

        private void Initialize()
        {
            _groupAddress = IPAddress.Parse(Address.Uri.Host);
            if (_groupAddress == null)
                throw new TransportException(Address.Uri, "The multicast address could not be determined");

            if (_groupAddress.AddressFamily != AddressFamily.InterNetwork)
                throw new TransportException(Address.Uri, "The specified address is not a valid multicast address");

            StartSender();
            StartReceiver();
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                StopReceiver();
                StopSender();
            }

            _disposed = true;
        }

        private void StartSender()
        {
            _sendIpEndPoint = new IPEndPoint(_groupAddress, Address.Uri.Port);

            const int port = 0;
            _sendClient = new UdpClient(port, AddressFamily.InterNetwork);
            _sendClient.DontFragment = true;
            _sendClient.Ttl = 2; // 0 = host, 1 = subnet, <32 = same company 
            _sendClient.Client.SendBufferSize = 256*1024;

            _sendClient.JoinMulticastGroup(_groupAddress);
        }

        private void StartReceiver()
        {
            _receiveClient = new UdpClient();

            Socket s = _receiveClient.Client;

            const int optionValue = 1;
            s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, optionValue);

            s.ReceiveBufferSize = 256*1024;
            s.ReceiveTimeout = 2000;
            s.Bind(new IPEndPoint(IPAddress.Any, Address.Uri.Port));

            _receiveClient.JoinMulticastGroup(_groupAddress, 2); // 0 = host, 1 = subnet, <32 = same company
        }

        private void StopSender()
        {
            if (_sendClient != null)
            {
                using (_sendClient)
                    _sendClient.Close();
            }
        }

        private void StopReceiver()
        {
            if (_receiveClient != null)
            {
                using (_receiveClient)
                {
                    _receiveClient.Close();
                }
            }
        }

        private ObjectDisposedException NewDisposedException()
        {
            return new ObjectDisposedException("The transport has already been disposed: " + Address);
        }

        ~MulticastUdpTransport()
        {
            Dispose(false);
        }
    }
}