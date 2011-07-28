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
