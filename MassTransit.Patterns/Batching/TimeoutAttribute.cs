namespace MassTransit.Patterns.Batching
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class TimeoutAttribute : Attribute
    {
        private TimeSpan _timeout;


        public TimeoutAttribute(TimeSpan timeout)
        {
            _timeout = timeout;
        }


        public TimeSpan Timeout
        {
            get { return _timeout; }
        }
    }
}