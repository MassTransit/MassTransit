Request Response
================

Thanks to the power of the async/await keywords in C# and that MassTransit fully
embraces the TPL we can now write very simple request / response code even though
we are using fully durable messaging platforms. Below is a SIMPLE example of
using the MT ``MessageRequestClient``.

.. sourcecode:: csharp

  var requestEndpoint = new Uri("loopback://localhost/input_queue");
  IRequestClient<SimpleCommand, SimpleCommandResult> commandClient =
                new MessageRequestClient<SimpleCommand, SimpleCommandResult>(busControl,
                    requestEndpoint, TimeSpan.FromSeconds(30));
  SimpleCommandResult result = await commandClient.Request(new SimpleCommand());
  //do stuff w/ result.

You can see that we get to keep using standard procedural code, and don't have to
fight callback hell.
