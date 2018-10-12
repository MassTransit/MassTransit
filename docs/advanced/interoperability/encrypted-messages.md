# Encrypted Messages

If you use the encrypted message serializer, it uses BSON under the hood. The encryption format is AES-256.

## BSON

Before any encryption occurs the message is serialized in to BSON using the same `BsonMessageSerializer` that is used when just the BSON message format is used.

## Initialization Vector (IV)

The IV is rotated on every message to ensure that identical messages encrypted with same symmetric key do not end up with the same ciphertext. The IV itself is not a secret, it acts as a salt and is written to the first 16 bytes of the encrypted message so that it can be used to decrypt the message on the other side.

## Symmetric Key

MassTransit uses a default built in `ConstantSecureKeyProvider` allowing you to use a constant key for encrypting all your messages, this takes in a array of bytes to be used when encrypting the messages.

However, if required you can implement your own `ISecureKeyProvider`, this is useful when you want to use a 3rd party key provider like AWS KMS or Azure Key Vault. Using a 3rd party like AWS KMS allows you to move the complexities of managing and rotating keys to someone else.

## Configuration

The encrypted message serializer needs to be configured on the publisher and the consumer so that both sides can understand the message, this is simply done when configuring the bus.

```csharp
var key = new []{ 1, 2, 3, 4, 5 };

var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
{
    var host = sbc.Host(new Uri("rabbitmq://localhost"), h =>
    {
        h.Username("guest");
        h.Password("guest");
    });

    sbc.UseEncryption(key)

    sbc.ReceiveEndpoint(host, "test_queue", ep =>
    {
        ep.Consumer<YourMessageConsumer>();
    });
});
```

You can also pass in your custom secure key provider in to the `UseEncryption` configuration method.

```csharp
var keyProvider = new CustomKeyProvider();

var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
{
    // ...

    sbc.UseEncryption(keyProvider)
});
```

### Unencrypted Message

After you've configured your encrypted message serializer, MassTransit will still process standard unencrypted messages. If this is undesirable then you can clear all other message deserializers on bus configuration.

```csharp
var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
{
    // ...
    sbc.ClearMessageDeserializers();
    sbc.UseEncryption(key)
});
```

