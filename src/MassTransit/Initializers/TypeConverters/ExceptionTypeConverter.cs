namespace MassTransit.Initializers.TypeConverters
{
    using System;
    using Events;
    using Util;


    public class ExceptionTypeConverter :
        ITypeConverter<string, Exception>,
        ITypeConverter<ExceptionInfo, Exception>
    {
        public bool TryConvert(Exception input, out string result)
        {
            if (input != null)
            {
                result = ExceptionUtil.GetMessage(input);
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(Exception input, out ExceptionInfo result)
        {
            if (input != null)
            {
                result = new FaultExceptionInfo(input);
                return true;
            }

            result = default;
            return false;
        }
    }
}