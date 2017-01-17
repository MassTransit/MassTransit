namespace MassTransit.Transports.InMemory.GraphValidation
{
    interface ITarjanNodeProperties
    {
        int Index { get; set; }
        int LowLink { get; set; }
    }
}