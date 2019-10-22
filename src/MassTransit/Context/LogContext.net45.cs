namespace MassTransit.Context
{
    using System;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Messaging;


    public partial class LogContext
    {
        public static ILogContext Current
        {
        #if ALLOW_PARTIALLY_TRUSTED_CALLERS
            [System.Security.SecuritySafeCriticalAttribute]
        #endif
            get
            {
                ObjectHandle logContextHandle = (ObjectHandle)CallContext.LogicalGetData(FieldKey);

                return (ILogContext)logContextHandle?.Unwrap();
            }

        #if ALLOW_PARTIALLY_TRUSTED_CALLERS
            [System.Security.SecuritySafeCriticalAttribute]
        #endif
            set
            {
                if (ValidateSetCurrent(value))
                {
                    SetCurrent(value);
                }
            }
        }

    #if ALLOW_PARTIALLY_TRUSTED_CALLERS
        [System.Security.SecuritySafeCriticalAttribute]
    #endif
        static void SetCurrent(ILogContext logContext)
        {
            CallContext.LogicalSetData(FieldKey, new ObjectHandle(logContext));
        }

        static readonly string FieldKey = $"{typeof(LogContext).FullName}_{AppDomain.CurrentDomain.Id}";
    }
}
