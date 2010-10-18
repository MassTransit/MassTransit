// Copyright 2007-2010 The Apache Software Foundation.
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
namespace MassTransit.Services.MessageLog
{
    using System;
    using System.IO;
    using Internal;
    using Pipeline;
    using Serialization.Custom;

    public class MessageLoggingService :
        IBusService
    {
        UnregisterAction _dispose;
        CustomXmlSerializer _serializer;

        public void Dispose()
        {
            //do nothing
        }

        public void Start(IServiceBus bus)
        {
            _serializer = new CustomXmlSerializer();
            _dispose = bus.OutboundPipeline.Filter<object>(m =>
            {
                //TODO: run in a fiber w/ an actor

                using (var stream = new MemoryStream())
                {
                    _serializer.Serialize(stream, m);
                    var data = stream.ToArray();
                    var log = new LogEntry
                                  {
                                      MessageType = m.GetType().FullName,
                                      RawMessage = data,
                                      ObservedAt = DateTime.Now
                                  };

                    //repo.save(log)    
                }

                return true;
            });
        }

        public void Stop()
        {
            _dispose();
        }
    }
}