# Encrypted Messages

If you use the encrypted message serializer, it uses BSON under the hood. The encryption format is AES-256.

## BSON

Before any encryption occurs the message is serialized in to BSON using the same `BsonMessageSerializer` that is used when just the BSON message format is used.

## Initialization Vector (IV)

The IV is rotated on every message to ensure that identical messages encrypted with same symmetric key do not end up with the same ciphertext. The IV itself is not a secret, it acts as a salt and is written to the first 16 bytes of the encrypted message so that it can be used to decrypt the message on the other side.

## Symmetric Key

MassTransit uses a default built in `ConstantSecureKeyProvider` allowing you to use a constant key for encrypting all your messages, this takes in a array of bytes to be used when encrypting the messages.

However, if required you can implement your own `ISecureKeyProvider`, this is useful when you want to use a 3rd party key provider like AWS KMS or Azure Key Vault. Using a 3rd party like AWS KMS allows you to move the complexities of managing and rotating keys to someone else.

### Generate a Key

The best way to generate a key is via C# Interactive. Start by dropping in to the _Developer Command Prompt for VS_, this can be achieved via the Start Menu or by executing the following from the Run Dialog (`[Windows]`+`[R]`).

```bash
cmd.exe /k "C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\Tools\VsDevCmd.bat"
```

Once in command prompt, run the C# Interactive (csi.exe) then run the following statements to generate a key.

```csharp
> using System.Security.Cryptography;
> var aes = new AesCryptoServiceProvider();
> aes.GenerateKey()
> aes.Key
byte[32] { 115, 171, 121, 43, 89, 24, 199, 205, 23, 221, 178, 104, 163, 32, 45, 84, 171, 86, 93, 13, 198, 132, 38, 65, 130, 192, 6, 159, 227, 104, 245, 222 }
```

If you want a text representation of the key, you can Base64 encode the key which can then be store in your `App.Config` or `appsettings.json`.

```csharp
> var base64Key = Convert.ToBase64String(aes.Key);
> base64Key
"c6t5K1kYx80X3bJooyAtVKtWXQ3GhCZBgsAGn+No9d4="
```

> Note that once you've generated the keys to keep them private!

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

