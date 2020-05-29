namespace MassTransit.Tests.Saga.Messages
{
    using System;


    [Serializable]
    public class CompleteSimpleSaga :
        SimpleSagaMessageBase
    {
        public CompleteSimpleSaga()
        {
        }

        public CompleteSimpleSaga(Guid correlationId)
            :
            base(correlationId)
        {
        }
    }
}
