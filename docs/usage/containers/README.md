# Configuring a container

When MassTransit was originally built, it required a dependency injection container to work. Unfortunately,
in 2008 there were few .NET developers who knew what Inversion of Control (IOC) or Dependency Injection (DI) was,
and even fewer who used it. Several versions later, the requirement was dropped and container support was put back
where it belonged - around the edges of the application instead of at the core of the framework.

Fortunately, the world has changed and DI is more mainstream, particularly with newer applications. And container
support in MassTransit has stayed up to date, including all of the major containers.

<div class="alert alert-info">
<b>Note:</b>
    Dependency Injection styles are a personal choice that each developer or organization must make on their
    own. We recognize this choice, and respect it, and will not judge those who don't use a particular container
    or style of dependency injection. In short, we care.
</div>

**Hey! Where is my container??**

Containers come and go, so if you don't see your container here, or feel that the support for you container is weaksauce,
pull requests are always welcome. Using an existing container it should be straight forward to add support for your favorite
ÜberContainer.

* [Autofac](autofac.md)
* [Ninject](ninject.md)
* [StructureMap](structuremap.md)
* [Unity](unity.md)
* [Castle Windsor](castlewindsor.md)
* [Microsoft Dependency Injection](msdi.md)
