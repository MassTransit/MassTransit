namespace MassTransit
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Util;


    /// <summary>
    /// For use by application developers to include additional data elements along with the exception, which will be
    /// transferred to the <see cref="ExceptionInfo" /> of the <see cref="Fault{T}" /> event.
    /// </summary>
    public class MassTransitApplicationException :
        Exception
    {
        Dictionary<string, object> _data;

        protected MassTransitApplicationException()
        {
        }

        public MassTransitApplicationException(Exception innerException)
            : base(innerException.Message, innerException)
        {
            ImportExceptionData(innerException);
        }

        public MassTransitApplicationException(Exception innerException, object values)
            : base(innerException.Message, innerException)
        {
            _data = ConvertObject.ToDictionary(values);

            ImportExceptionData(innerException);
        }

        public MassTransitApplicationException(Exception innerException, IEnumerable<KeyValuePair<string, object>> values)
            : base(innerException.Message, innerException)
        {
            _data = values.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);

            ImportExceptionData(innerException);
        }

        public MassTransitApplicationException(string message, Exception innerException)
            : base(message, innerException)
        {
            ImportExceptionData(innerException);
        }

        public MassTransitApplicationException(string message, Exception innerException, object values)
            : base(message, innerException)
        {
            _data = ConvertObject.ToDictionary(values);

            ImportExceptionData(innerException);
        }

        public MassTransitApplicationException(string message, Exception innerException, IEnumerable<KeyValuePair<string, object>> values)
            : base(message, innerException)
        {
            _data = values.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);

            ImportExceptionData(innerException);
        }

        public override IDictionary Data => _data ?? base.Data;

        public IDictionary<string, object> ApplicationData => _data;

        void ImportExceptionData(Exception exception)
        {
            if (exception.Data == null)
                return;

            var keys = exception.Data.Keys;
            if (keys.Count == 0)
                return;

            foreach (var key in keys)
            {
                if (key is string stringKey && (_data == null || !_data.ContainsKey(stringKey)))
                {
                    var value = exception.Data[key];
                    if (value != null)
                    {
                        _data ??= new Dictionary<string, object>();

                        _data.Add(stringKey, value);
                    }
                }
            }
        }
    }
}
