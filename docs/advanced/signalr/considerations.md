# Considerations

* [Sticky Sessions is required, unless you force Websockets only](https://github.com/aspnet/SignalR/issues/2002#issuecomment-383622076)
  * [Also a good read](https://rolandguijt.com/scaling-out-your-asp-net-core-signalr-application/)
* Although [this page](https://docs.microsoft.com/en-us/aspnet/signalr/overview/performance/scaleout-in-signalr) is written for the old SignalR, the scaleout concepts still apply.
* Having a single hub is fine, but only use multiple hubs [for organization, not performance](https://stackoverflow.com/a/22151160).