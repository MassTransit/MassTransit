namespace MassTransit
{
    using System;


    [Serializable]
    public enum ValidationResultDisposition
    {
        Success,
        Warning,
        Failure,
    }
}
