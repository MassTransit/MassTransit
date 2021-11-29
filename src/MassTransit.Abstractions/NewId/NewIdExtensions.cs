namespace MassTransit
{
    using System;


    public static class NewIdExtensions
    {
        public static NewId ToNewId(this Guid guid)
        {
            return NewId.FromGuid(guid);
        }

        public static NewId ToNewIdFromSequential(this Guid guid)
        {
            return NewId.FromSequentialGuid(guid);
        }
    }
}
