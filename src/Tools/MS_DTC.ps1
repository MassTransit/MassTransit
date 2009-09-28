$sc = Get-Service -name MSDTC
if($sc.Status -eq "Running")
{
	Write-Host "The MSDTC is running. Good"
}
else
{
	Write-Error "The MSDTC must be running and is currently $sc.Status"
}

$security = Get-Item HKLM:\Software\Microsoft\MSDTC\Security

$NetworkDtcAccess = $security.GetValue("NetworkDtcAccess")
$NetworkDtcAccessAdmin = $security.GetValue("NetworkDtcAccessAdmin")
$NetworkDtcAccessClients = $security.GetValue("NetworkDtcAccessClients")
$NetworkDtcAccessTransactions = $security.GetValue("NetworkDtcAccessTransactions")
$NetworkDtcAccessInbound = $security.GetValue("NetworkDtcAccessInbound")
$NetworkDtcAccessOutbound = $security.GetValue("NetworkDtcAccessOutbound")
$NetworkDtcAccessTip = $security.GetValue("NetworkDtcAccessTip")
$XaTransactions = $security.GetValue("XaTransactions")
$DomainControllerState = $security.GetValue("DomainControllerState")
$AccountName = $security.GetValue("AccountName")

Write-Host "Actions Needed"
if($NetworkDtcAccess -eq 0)
{
	Write-Warning "Network DTC Access is off. Please turn on"
}
if($NetworkDtcAccessAdmin -eq 0)
{
	Write-Warning "Remote DTC Admin is off. Please turn on"
}
if($NetworkDtcAccessClients -eq 0)
{
	Write-Warning "Remote DTC Client is off. Please turn on"
}
if($NetworkDtcAccessInbound -eq 0)
{
	Write-Warning "Inbound Transaction Communication is off. Please turn on"
}
if($NetworkDtcAccessOutbound -eq 0)
{
	Write-Warning "Outbound Transaction Communication is off. Please turn on"
}
if($NetworkDtcAccessTip -ne 0)
{
	Write-Warning "Do you really need TIP Transactions on?"
}
if($XaTransactions -ne 0)
{
	Write-Warning "Do you really need XA Transactions on?"
}