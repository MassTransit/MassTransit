Turning on Diagnostics
""""""""""""""""""""""

If you want to get a snapshot of how your service bus is configured, you can get
a pretty good picture of it by using the method.

.. sourcecode:: csharp

  var bus = Bus.Factory.CreateUsingInMemory(cfg => { /* usual stuff */ });
  var probe = bus.Probe();
  //you can now inspect the probe

  //for your convience we also have added a few helper methods.
  bus.WriteIntrospectionToFile("a_file.txt"); //great to send with support requests :)
  bus.WriteIntrospectionToConsole();

You may also want to inspect a running bus instance remotely. For that you just need to enable
remote introspection like so.

.. sourcecode:: csharp

  Bus.Factory.CreateUsingInMemory(cfg =>
  {
      //the usual options

    cfg.EnableRemoteInstrospection();
  });

You can then use the ``busdriver`` to query the status. using:

  busdriver status -uri:<address to control bus>
