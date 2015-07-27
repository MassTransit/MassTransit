Send Only Bus
=============

.. sourcecode:: csharp

    var readOnlyBus = Bus.Factory.CreateUsingInMemory(cfg => {});
    readOnlyBus.Publish(yourMessage);
