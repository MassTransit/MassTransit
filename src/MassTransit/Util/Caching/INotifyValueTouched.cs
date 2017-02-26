namespace MassTransit.Util.Caching
{
    using System;


    public interface INotifyValueTouched
    {

        event Action Touched;
    }
}