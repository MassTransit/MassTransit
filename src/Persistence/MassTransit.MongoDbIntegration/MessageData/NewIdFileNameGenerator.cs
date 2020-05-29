namespace MassTransit.MongoDbIntegration.MessageData
{
    using Util;


    public class NewIdFileNameGenerator :
        IFileNameGenerator
    {
        public string GenerateFileName()
        {
            return FormatUtil.Formatter.Format(NewId.Next().ToSequentialGuid().ToByteArray());
        }
    }
}
