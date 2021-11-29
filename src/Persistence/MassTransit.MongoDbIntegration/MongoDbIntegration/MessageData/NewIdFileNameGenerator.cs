namespace MassTransit.MongoDbIntegration.MessageData
{
    using NewIdFormatters;


    public class NewIdFileNameGenerator :
        IFileNameGenerator
    {
        public string GenerateFileName()
        {
            return ZBase32Formatter.LowerCase.Format(NewId.Next().ToSequentialGuid().ToByteArray());
        }
    }
}
