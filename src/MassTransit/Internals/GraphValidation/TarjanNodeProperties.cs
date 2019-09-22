namespace MassTransit.Internals.GraphValidation
{
    interface ITarjanNodeProperties
    {
        int Index { get; set; }
        int LowLink { get; set; }
    }
}