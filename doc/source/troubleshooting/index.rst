Troubleshooting MassTransit
"""""""""""""""""""""""""""


Issues and possible solutions
'''''''''''''''''''''''''''''''''''''''''''''''''
* Messages are not being received by bus instances
Possible solutions 
,,,,,,,,,,,,,,,,,,
1) Assuming bus instances are configured to use MSMQ transport and are using multicast. 
Machines with multiple NICs (either hardware or virtual devices) need to have MSMQ binding configured in the following way :

     
At the registry key [HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\MSMQ\Parameters]

* Add new string value with name "BindInterfaceIP". As a value enter machine's IP address
* Add new string value with name "MulticastBindIP". As a value enter machine's IP address
* Restart MSMQ service

At the end the registry entries should look like this : 

.. image :: http://i1166.photobucket.com/albums/q612/myarichuk/MSMQMulticasting.jpg

References

* http://support.microsoft.com/kb/974813
* http://technet.microsoft.com/en-us/library/cc756156(v=ws.10).aspx

Confirmed to work in Windows Vista SP2 and Windows Server 2008 R2 environments