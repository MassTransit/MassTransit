namespace MassTransit.Events
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Util;


    [Serializable]
    public class FaultExceptionInfo :
        ExceptionInfo
    {
        public FaultExceptionInfo()
        {
        }

        public FaultExceptionInfo(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            if (exception.Data is IDictionary<string, object> dictionary)
                Data = dictionary;
            else if (exception.Data != null)
                UpdateData(exception.Data);

            if (exception is MassTransitApplicationException { InnerException: { } })
            {
                exception = exception.InnerException;

                if (exception.Data != null)
                    UpdateData(exception.Data);
            }

            ExceptionType = TypeCache.GetShortName(exception.GetType());
            InnerException = exception.InnerException != null
                ? new FaultExceptionInfo(exception.InnerException)
                : null;

            StackTrace = ExceptionUtil.GetStackTrace(exception);
            Message = ExceptionUtil.GetMessage(exception);
            Source = exception.Source;
        }

        public string ExceptionType { get; set; }

        public ExceptionInfo InnerException { get; set; }

        public string StackTrace { get; set; }

        public string Message { get; set; }
        public string Source { get; set; }

        public IDictionary<string, object> Data { get; set; }

        void UpdateData(IDictionary dictionary)
        {
            var keys = dictionary.Keys;
            if (keys.Count == 0)
                return;

            foreach (var key in keys)
            {
                if (key is string stringKey && (Data == null || !Data.ContainsKey(stringKey)))
                {
                    var value = dictionary[key];
                    if (value != null)
                    {
                        Data ??= new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                        Data.Add(stringKey, value);
                    }
                }
            }
        }
    }
}
