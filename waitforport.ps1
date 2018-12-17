param ([int]$port, [int]$maxAttempts = 5)
[int]$attempt=0
while(!$client.Connected -and $attempt -lt $maxAttempts) {
  try {
    $client = New-Object System.Net.Sockets.TcpClient([System.Net.Sockets.AddressFamily]::InterNetwork)
    $attempt++; $client.Connect("127.0.0.1", $port); write-host "service is listening on port $port"
  }
  catch {
    [int]$sleepTime = 10*$attempt
    if ($attempt -lt $maxAttempts) {      
      write-host "Service is not listening on port $port. Retry after $sleepTime seconds..."
      sleep $sleepTime;
      $client.Close();  
    }
  }  
}
if (!$client.Connected) {
throw "unable to make service listen in $maxAttempts attempst"
}
$client.Close()