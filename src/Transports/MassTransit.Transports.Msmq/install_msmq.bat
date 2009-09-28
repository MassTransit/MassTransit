;@ECHO OFF
;sysocmgr.exe /i:sysoc.inf /u:InstallMsmq.bat
;GOTO Finished

[Components]
msmq_Core = ON
msmq_LocalStorage = ON
msmq_ADIntegrated = OFF
msmq_TriggersService = OFF
msmq_HTTPSupport = OFF
msmq_RoutingSupport = OFF
msmq_MQDSService = OFF

;:Finished 