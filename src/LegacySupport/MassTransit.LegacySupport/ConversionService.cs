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
namespace MassTransit.Services.LegacyProxy
{
    using System;
    using Internal;
    using log4net;
    using Magnum.Extensions;
    using Magnum.Pipeline.Segments;
    using Magnum.Reflection;
    using Threading;

    public class ConversionService
    {
        static readonly ILog _log = LogManager.GetLogger(typeof (ConversionService));
        readonly IEndpointFactory _ef;
        IEndpoint _oldEndpoint;
        IEndpoint _legacyServiceEndpoint;
        ConsumerPool _pool;
        readonly Configuration _config;

        public ConversionService(IEndpointFactory ef, Configuration config)
        {
            _ef = ef;
            _config = config;
        }

        public void Start()
        {
            _oldEndpoint = _ef.GetEndpoint(_config.LegacyServiceProxyUri);
            _legacyServiceEndpoint = _ef.GetEndpoint(_config.LegacyServiceDataUri);
            var fac = new OldMessageFactory();
            Action<object> todo = msg =>
                                   {
                                       try
                                       {
                                           var conv = fac.ConvertOldToNew(msg);
                                           _log.DebugFormat("Converted '{0}' to '{1}'", msg.GetType().Name, conv.GetType().Name);
                                           _legacyServiceEndpoint.FastInvoke("Send", conv);
                                       }
                                       catch (Exception ex)
                                       {
                                           _log.Error(ex);
                                           throw;
                                       }
                                   };

            _pool = new EndpointThreadPoolConsumerPool(_oldEndpoint, PipeSegment.New(), 3.Seconds(), todo)
                        {
                            MaximumConsumerCount = 1
                        };
            _pool.Start();
        }

        public void Stop()
        {
            if (_pool != null)
            {
                _pool.Stop();
                _pool.Dispose();
                _pool = null;
            }
            _oldEndpoint = null;
            _legacyServiceEndpoint = null;
        }
    }
}