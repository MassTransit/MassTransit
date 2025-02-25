namespace MassTransit;

using System;


[Serializable]
public class RecurringJobException :
    MassTransitException
{
    public RecurringJobException()
    {
    }

    public RecurringJobException(string message)
        : base(message)
    {
    }
}
