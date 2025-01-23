namespace MassTransit
{
    using System;
    using System.Linq;
    using System.Runtime.Serialization;


    [Serializable]
    public class RequestFaultException :
        RequestException
    {
        public RequestFaultException(string requestType, Fault fault)
            : base($"The {requestType} request faulted: {string.Join(Environment.NewLine, fault.Exceptions?.Select(x => x.Message) ?? [])}")
        {
            RequestType = requestType;
            Fault = fault;
        }

        public RequestFaultException()
        {
        }

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected RequestFaultException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            RequestType = info.GetString("RequestType");
            Fault = info.GetValue("Fault", typeof(Fault)) as Fault;
        }

        public string? RequestType { get; private set; }
        public Fault? Fault { get; private set; }

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("RequestType", RequestType);
            info.AddValue("Fault", Fault);
        }
    }
}
