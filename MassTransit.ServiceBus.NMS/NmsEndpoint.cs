/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.
namespace MassTransit.ServiceBus.NMS
{
    using System;
    using Internal;

    public class NmsEndpoint :
        INmsEndpoint
    {
        private IMessageReceiver _receiver;
        private IMessageSender _sender;
        private Uri _uri;

        public NmsEndpoint(Uri uri)
        {
            _uri = uri;
        }

        public NmsEndpoint(string uriString)
        {
            _uri = new Uri(uriString);
        }

        #region INmsEndpoint Members

        public Uri Uri
        {
            get { return _uri; }
        }

        public IMessageSender Sender
        {
            get
            {
                lock (this)
                {
                    if (_sender == null)
                        _sender = new NmsMessageSender(this);
                }
                return _sender;
            }
        }

        public IMessageReceiver Receiver
        {
            get
            {
                lock (this)
                {
                    if (_receiver == null)
                        _receiver = new NmsMessageReceiver(this);
                }

                return _receiver;
            }
        }

        public void Dispose()
        {
            if (_receiver != null)
                _receiver.Dispose();

            if (_sender != null)
                _sender.Dispose();
        }

        #endregion
    }
}