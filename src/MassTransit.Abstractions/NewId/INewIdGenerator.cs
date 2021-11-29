namespace MassTransit
{
    using System;


    public interface INewIdGenerator
    {
        NewId Next();

        ArraySegment<NewId> Next(NewId[] ids, int index, int count);
        Guid NextGuid();
        Guid NextSequentialGuid();
    }
}
