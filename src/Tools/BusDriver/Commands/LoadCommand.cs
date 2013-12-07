// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace BusDriver.Commands
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Formatting;
    using Magnum.Extensions;
    using Magnum.FileSystem;
    using Magnum.FileSystem.Internal;
    using MassTransit;
    using MassTransit.Context;
    using MassTransit.Exceptions;
    using MassTransit.Logging;
    using MassTransit.Transports;


    public class LoadCommand :
        Command
    {
        static readonly ILog _log = Logger.Get(typeof(LoadCommand));
        readonly int _count;
        readonly string _name;
        readonly bool _remove;
        readonly string _uriString;

        public LoadCommand(string uriString, string name, int count, bool remove)
        {
            _uriString = uriString;
            _name = name;
            _count = count;
            _remove = remove;
        }

        public bool Execute()
        {
            Uri uri = _uriString.ToUri("The from URI was invalid");

            AbsolutePathName fullPath = PathName.GetAbsolutePathName(_name, Environment.CurrentDirectory);
            _log.DebugFormat("Using output path name: {0}", fullPath);

            string directoryName = Path.GetDirectoryName(fullPath.GetPath());
            if (!System.IO.Directory.Exists(directoryName))
                System.IO.Directory.CreateDirectory(directoryName);

            IOutboundTransport toTransport = Program.Transports.GetOutboundTransport(uri);

            ITextBlock text = new TextBlock()
                .BeginBlock("Load messages to URI: " + uri, "");

            string[] files =
                System.IO.Directory.GetFiles(directoryName, fullPath.GetName() + "*.msg", SearchOption.TopDirectoryOnly)
                      .OrderBy(x => x).ToArray();

            int loadCount = 0;
            for (int i = 0; i < files.Length && loadCount < _count; i++)
            {
                string file = files[i];

                string fileName = Path.Combine(directoryName, file);

                text.BodyFormat("Message File: {0}", file);

                ISendContext context = LoadMessageFromFile(fileName);

                toTransport.Send(context);

                if (_remove)
                    System.IO.File.Delete(fileName);

                loadCount++;
            }

            _log.Info(text.ToString());

            return true;
        }

        ISendContext LoadMessageFromFile(string fileName)
        {
            using (StreamReader stream = System.IO.File.OpenText(fileName))
            {
                string contentType = null;

                string line;
                do
                {
                    line = stream.ReadLine();
                    if (line != null)
                    {
                        string[] values = line.Split(':');
                        if (values.Length == 2)
                        {
                            if (values[0] == "Content-Type")
                                contentType = values[1].Trim();
                        }
                    }
                }
                while (line.IsNotEmpty());

                string body = stream.ReadToEnd();
                stream.Close();

                return new LoadMessageSendContext(contentType, Encoding.UTF8.GetBytes(body));
            }
        }


        class LoadMessageSendContext :
            MessageContext,
            ISendContext
        {
            readonly byte[] _body;
            readonly Action<Stream> _bodyWriter;

            public LoadMessageSendContext(string contentType, byte[] body)
            {
                SetContentType(contentType);
                _body = body;

                _bodyWriter = stream => stream.Write(_body, 0, _body.Length);
            }

            public Guid Id { get; set; }

            public Type DeclaringMessageType
            {
                get { return typeof(object); }
            }

            public void SetDeliveryMode(DeliveryMode deliveryMode)
            {
                DeliveryMode = deliveryMode;
            }

            public DeliveryMode DeliveryMode { get; private set; }

            public void SerializeTo(Stream stream)
            {
                _bodyWriter(stream);
            }

            public bool TryGetContext<T>(out IBusPublishContext<T> context)
                where T : class
            {
                throw new MessageException(typeof(T), "The message type is unknown and can not be type-cast");
            }

            public void NotifySend(IEndpointAddress address)
            {
            }
        }
    }
}