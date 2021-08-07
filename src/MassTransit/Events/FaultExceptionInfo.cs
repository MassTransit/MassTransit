namespace MassTransit.Events
{
    using System;
    using System.Collections.Generic;
    using Metadata;
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

            if (exception is MassTransitApplicationException applicationException)
            {
                Data = applicationException.ApplicationData;

                if (applicationException.InnerException != null)
                    exception = applicationException.InnerException;
            }

            ExceptionType = TypeMetadataCache.GetShortName(exception.GetType());
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
    }
}
