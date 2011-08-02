Msmq Configuration Options
""""""""""""""""""""""""""""""

.. sourcecode:: csharp

  ServiceBusFactory.New(sbc => 
  {
    sbc.UseMsmq();
    sbc.VerifyMsmqConfiguratio();
    sbc.VerifyMsDtcConfiguration():
  });


Crap. I just wiped out the msmq and have to rewrite.

``UseMsmq()`` should be obvious. It configures the bus to use the MSMQ transport.

``VerifyMsmqConfiguration()`` will confirm that MSMQ is correctly installed and fix
the install by adding missing components.

``VerifyMsDtcConfiguration()`` will confirm that the DTC is setup correctly.
