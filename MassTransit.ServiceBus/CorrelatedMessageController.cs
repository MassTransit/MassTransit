using System.Collections.Generic;
using System.Reflection;
using log4net;
using MassTransit.ServiceBus.Util;

namespace MassTransit.ServiceBus
{
    public class CorrelatedMessageController
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Dictionary<MessageId, ServiceBusAsyncResult> _asyncResultDictionary =
            new Dictionary<MessageId, ServiceBusAsyncResult>();

        public ServiceBusAsyncResult Track(MessageId id)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Recording request correlation ID {0}", id);

            ServiceBusAsyncResult asyncResult = new ServiceBusAsyncResult();

            _asyncResultDictionary.Add(id, asyncResult);

            return asyncResult;
        }

        public Result Match(MessageId id)
        {
            return _asyncResultDictionary.ContainsKey(id);
        }

        public Result Process(IEnvelope envelope)
        {
            if (_asyncResultDictionary.ContainsKey(envelope.CorrelationId))
            {
                ServiceBusAsyncResult asyncResult = _asyncResultDictionary[envelope.CorrelationId];
                _asyncResultDictionary.Remove(envelope.CorrelationId);

                asyncResult.Complete(envelope.Messages);

                return true;
            }

            return false;
        }

        public class Result
        {
            private readonly bool _result;

            private Result(bool result)
            {
                _result = result;
            }

            public bool Found
            {
                get { return _result; }
            }

            public bool WasHandled
            {
                get { return _result; }
            }

            public static implicit operator Result(bool result)
            {
                return new Result(result);
            }
        }
    }
}