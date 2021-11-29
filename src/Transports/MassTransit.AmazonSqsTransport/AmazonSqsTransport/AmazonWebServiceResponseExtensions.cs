namespace MassTransit.AmazonSqsTransport
{
    using System.Net;
    using Amazon.Runtime;


    public static class AmazonWebServiceResponseExtensions
    {
        public static void EnsureSuccessfulResponse(this AmazonWebServiceResponse response)
        {
            const string documentationUri = "https://aws.amazon.com/blogs/developer/logging-with-the-aws-sdk-for-net/";

            var statusCode = response.HttpStatusCode;
            var requestId = response.ResponseMetadata.RequestId;

            if (statusCode >= HttpStatusCode.OK && statusCode < HttpStatusCode.MultipleChoices)
                return;

            throw new AmazonSqsTransportException(
                $"Received unsuccessful response ({statusCode}) from AWS endpoint. See AWS SDK logs ({requestId}) for more details: {documentationUri}");
        }
    }
}
