// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Pipeline.Sinks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Magnum.Concurrency;

    /// <summary>
    /// Maintains a dictionary of valid values for the specified string expression
    /// </summary>
    /// <typeparam name="TMessage">The type of the message to be routed</typeparam>
    /// <typeparam name="T"></typeparam>
    public class RequestMessageRouter<T, TMessage> :
        IPipelineSink<T>
        where TMessage : class
        where T : class, IMessageContext<TMessage>
    {
        readonly IEnumerable<Action<T>> _empty;
        readonly Func<T, string> _keyAccessor;
        readonly Atomic<Dictionary<string, MessageRouter<T>>> _output;

        public RequestMessageRouter(Func<T, string> keyAccessor)
        {
            _keyAccessor = keyAccessor;
            _output = Atomic.Create(new Dictionary<string, MessageRouter<T>>());
            _empty = Enumerable.Empty<Action<T>>();
        }

        public IEnumerable<Action<T>> Enumerate(T context)
        {
            string key = _keyAccessor(context);
            if (!string.IsNullOrEmpty(key))
            {
                MessageRouter<T> router;
                if (_output.Value.TryGetValue(key, out router))
                    return router.Enumerate(context);
            }

            return _empty;
        }

        public bool Inspect(IPipelineInspector inspector)
        {
            return inspector.Inspect(this, () => _output.Value.Values.All(x => x.Inspect(inspector)));
        }

        public int SinkCount(string key)
        {
            MessageRouter<T> router;
            if (!_output.Value.TryGetValue(key, out router))
                return 0;

            return router.SinkCount;
        }

        public UnsubscribeAction Connect(string key, IPipelineSink<T> sink)
        {
            _output.Set(sinks =>
                {
                    MessageRouter<T> router;
                    if (sinks.TryGetValue(key, out router) == false)
                    {
                        router = new MessageRouter<T>();
                        router.Connect(sink);

                        return new Dictionary<string, MessageRouter<T>>(sinks) {{key, router}};
                    }

                    var result = new Dictionary<string, MessageRouter<T>>(sinks);

                    router = new MessageRouter<T>(router.Sinks);
                    router.Connect(sink);
                    result[key] = router;

                    return result;
                });

            return () => Disconnect(key, sink);
        }

        bool Disconnect(string key, IPipelineSink<T> sink)
        {
            return _output.Set(sinks =>
                {
                    MessageRouter<T> router;
                    if (sinks.TryGetValue(key, out router) == false)
                        return sinks;

                    var result = new Dictionary<string, MessageRouter<T>>(sinks);

                    List<IPipelineSink<T>> outputSinks = router.Sinks.Where(x => x != sink).ToList();
                    if (outputSinks.Count == 0)
                        result.Remove(key);
                    else
                        result[key] = new MessageRouter<T>(outputSinks);

                    return result;
                }) != null;
        }
    }
}