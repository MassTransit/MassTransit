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
            : base($"The {requestType} request faulted: {string.Join(Environment.NewLine, fault.Exceptions.Select(x => x.Message))}")
        {
            RequestType = requestType;
            Fault = fault;
        }

        public RequestFaultException()
        {
        }

        protected RequestFaultException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            RequestType = info.GetString("RequestType");
            Fault = (Fault)info.GetValue("Fault", typeof(Fault));
        }

        public string? RequestType { get; private set; }
        public Fault? Fault { get; private set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("RequestType", RequestType);
            info.AddValue("Fault", Fault);
        }
    }
}
