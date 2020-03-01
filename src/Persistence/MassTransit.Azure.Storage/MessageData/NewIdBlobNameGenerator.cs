namespace MassTransit.Azure.Storage.MessageData
{
    using Util;


    public class NewIdBlobNameGenerator :
        IBlobNameGenerator
    {
        public string GenerateBlobName()
        {
            return FormatUtil.Formatter.Format(NewId.Next().ToSequentialGuid().ToByteArray());
        }
    }
}
