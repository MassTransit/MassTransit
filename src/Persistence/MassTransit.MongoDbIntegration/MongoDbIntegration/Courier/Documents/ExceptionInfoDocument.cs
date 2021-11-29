namespace MassTransit.MongoDbIntegration.Courier.Documents
{
    public class ExceptionInfoDocument
    {
        public ExceptionInfoDocument(ExceptionInfo exceptionInfo)
        {
            ExceptionType = exceptionInfo.ExceptionType;
            Message = exceptionInfo.Message;
            Source = exceptionInfo.Source;
            StackTrace = exceptionInfo.StackTrace;

            if (exceptionInfo.InnerException != null)
                InnerException = new ExceptionInfoDocument(exceptionInfo.InnerException);
        }

        public string ExceptionType { get; private set; }
        public string Message { get; private set; }
        public string Source { get; private set; }
        public string StackTrace { get; private set; }
        public ExceptionInfoDocument InnerException { get; private set; }
    }
}
