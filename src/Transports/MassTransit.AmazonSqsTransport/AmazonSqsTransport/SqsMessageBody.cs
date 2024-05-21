#nullable enable
namespace MassTransit.AmazonSqsTransport;

using System.Text.Json;
using Amazon.SQS.Model;


public class SqsMessageBody :
    StringMessageBody,
    JsonMessageBody
{
    readonly Message _message;
    JsonElement? _topicArn;

    public SqsMessageBody(Message message)
        : base(message.Body)
    {
        _message = message;
    }

    public string? TopicArn => _topicArn?.GetString();

    public JsonElement? GetJsonElement(JsonSerializerOptions options)
    {
        if (_message.Body == null)
            return null;

        var jsonElement = JsonSerializer.Deserialize<JsonElement>(_message.Body, options);

        if (jsonElement.TryGetProperty("TopicArn", out var topicElement) || jsonElement.TryGetProperty("topicArn", out topicElement))
        {
            _topicArn = topicElement;

            if (jsonElement.TryGetProperty("Message", out var messageElement) || jsonElement.TryGetProperty("message", out messageElement))
                return JsonSerializer.Deserialize<JsonElement>(messageElement.GetString() ?? string.Empty, options);
        }

        return jsonElement;
    }
}
