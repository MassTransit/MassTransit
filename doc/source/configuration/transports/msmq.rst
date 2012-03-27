Msmq Configuration Options
""""""""""""""""""""""""""""""

.. sourcecode:: csharp

  ServiceBusFactory.New(sbc => 
  {
    sbc.UseMsmq();
    sbc.VerifyMsmqConfiguration();
    sbc.VerifyMsDtcConfiguration():
  });


Crap. I just wiped out the msmq and have to rewrite.

``UseMsmq()`` should be obvious. It configures the bus to use the MSMQ transport.

``VerifyMsmqConfiguration()`` will confirm that MSMQ is correctly installed and fix
the install by adding missing components.

``VerifyMsDtcConfiguration()`` will confirm that the DTC is setup correctly.


.. note::

    A post on MSMQ and Ports: http://blogs.msdn.com/b/johnbreakwell/archive/2008/04/29/clear-the-way-msmq-coming-through.aspx

.. note::

    A post on public vs private queues: http://technet.microsoft.com/en-us/library/cc776346.aspx

Installing MSMQ Step by step (12/21/2011)
#. Get to 'Computer Management'
#. Expand 'Services and Applications'
#. Expand 'Message Queuing'
#. Right click on 'Private Queues': New > Private Queue
#. Enter a 'Queue Name'
#. Choose whether or not you would like it to be 'Transactional' or not.

.. note::

    Creating a queue this way will require permission changes. As you will be the only person able to administer the queue! STRONGLY suggested that you at this time give the Administrator role Full Control over the queue.


=========================  ==============  ============
Permission                 Role: Consumer  Role: Sender
=========================  ==============  ============
Full Control                -               -
Delete                      -               -
Receive Message             Y               -
Peek Message                Y               -
Receive Journal Message     -               -
Get Properties              Y               Y
Set Porperties              -               -
Get Permissions             Y               Y
Set Permissions             -               -
Take Ownership              -               -
Send Message                Y               Y
Special Permissions         -               -
=========================  ==============  ============