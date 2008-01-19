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

using System.Collections.Generic;
using System.Reflection;
using log4net;
using MassTransit.ServiceBus.Util;

namespace MassTransit.ServiceBus
{
    /// <summary>
    /// Dispatches replies to tracked MessageIds for asynchronous processing of the request/reply pattern
    /// </summary>
    public class AsyncReplyDispatcher
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Dictionary<MessageId, ServiceBusAsyncResult> _asyncResultDictionary =
            new Dictionary<MessageId, ServiceBusAsyncResult>();

        /// <summary>
        /// Adds a MessageId that will be tracked for responses
        /// </summary>
        /// <param name="id">The id of the message</param>
        /// <returns>An <c ref="IAsyncResult" /> compatible result that can be used for waiting on a response</returns>
        public ServiceBusAsyncResult Track(MessageId id)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Recording request correlation ID {0}", id);

            ServiceBusAsyncResult asyncResult = new ServiceBusAsyncResult();

            _asyncResultDictionary.Add(id, asyncResult);

            return asyncResult;
        }

        /// <summary>
        /// Removes a message id from the tracking list
        /// </summary>
        /// <param name="id"></param>
        public void Remove(MessageId id)
        {
            if (_asyncResultDictionary.ContainsKey(id))
            {
                _asyncResultDictionary.Remove(id);
            }
        }

        /// <summary>
        /// Checks an id to a MessageId that has been registered
        /// </summary>
        /// <param name="id">The id to match</param>
        /// <returns>Returns a <c ref="Result" /> that can be checked to see if the match was found</returns>
        public bool Exists(MessageId id)
        {
            return _asyncResultDictionary.ContainsKey(id);
        }

        /// <summary>
        /// Complete an outstanding asynchronous method using the specified envelope
        /// </summary>
        /// <param name="envelope">The envelope to use with the completion</param>
        /// <returns>Returns a <c ref="Result" /> that can be checked to see if the response WasHandled</returns>
        public bool Complete(IEnvelope envelope)
        {
            ServiceBusAsyncResult asyncResult = null;
            
            lock (_asyncResultDictionary)
            {
                if (_asyncResultDictionary.ContainsKey(envelope.CorrelationId))
                {
                    asyncResult = _asyncResultDictionary[envelope.CorrelationId];
                }
            }

            if (asyncResult != null)
            {
                _asyncResultDictionary.Remove(envelope.CorrelationId);

                asyncResult.Complete(envelope.Messages);

                return true;
            }

            return false;
        }
    }
}