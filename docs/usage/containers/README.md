# Configuring a container

MassTransit does not require a dependency injection container, however, it includes support for the most popular containers. Each container has its own style, and as such usage scenarios may vary.

MassTransit now has a consistent API for registration and configuration when using a container, as well as conventions for configuring receive endpoints based upon the registered consumers, routing slip activities, and sagas. Under the hood, each container is configured to properly interact with MassTransit, leveraging available container features, such as nested scopes for message consumers, without requiring the developer to explictly configure every consumer.

<div class="alert alert-info">
<b>Note:</b>
    Dependency Injection styles are a personal choice that each developer or organization must make on their own. We recognize this choice, and respect it, and will not judge those who don't use a particular container or style of dependency injection. In short, we care.
</div>

**Hey! Where is my container??**

Containers come and go, so if you don't see your container here, or feel that the support for you container is weaksauce, pull requests are always welcome. Using an existing container it should be straight forward to add support for your favorite ÜberContainer.

* [Autofac](autofac.md)
* [Ninject](ninject.md)
* [StructureMap](structuremap.md)
* [Lamar](lamar.md)
* [Unity](unity.md)
* [Castle Windsor](castlewindsor.md)
* [Microsoft Dependency Injection](msdi.md)
