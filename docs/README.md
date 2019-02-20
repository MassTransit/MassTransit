# What is MassTransit?

MassTransit is a free, open-source distributed application framework for .NET. MassTransit makes it easy to create applications and services that leverage message-based, loosely-coupled asynchronous communication for higher availability, reliability, and scalability.

MassTransit sits on top of several widely supported message transports and provides an [extensive set of features](understand/additions-to-transport.md) resulting in a developer-friendly approach to build asynchronous services using message-based conversation patterns. Message-based communication is a reliable and scalable way to implement a service oriented architecture.

## Getting started

To get started using MassTransit, check out the [Quick Start](quickstart.md).

## Getting help

MassTransit has a simple and approachable API, however, distributed application development is inherently complex. If you have questions, concerns, or issues using MassTransit, please visit the [Getting help](introduction/getting-help.md) page to find a list of useful resources.

## A bit of the back story? {#a-bit-of-the-backstory}

We are often asked why MassTransit was created, well here’s the story. :\)

In 2007, Chris Patterson \(@phatboyg\) and Dru Sellers \(@drusellers\) met at the first ALT.NET conference in Austin, TX. It was at this conference that Chris and Dru not only realized that they had a lot of the same problems to solve, but also how much the standard tooling provided by Microsoft just didn't fit their needs. Surrounded by the best and brightest in .NET, the energy was there to build better tooling that supported testable processes. Combined with an awareness of the latest advances in tooling, libraries, and coding practices; they decided that a better option must exist. After searching the .NET ecosystem for a tool that would help them achieve their goals, the only real option was the venerable NServiceBus. After reviewing NServiceBus, it was determined that the only real dependency injection container supported was Spring.NET. It also became obvious that NServiceBus wasn't quite ready for external contributors to come onboard. For these reasons, they decided to embark on the quixotic trek of building their own service bus \(seriously, how hard could it be?? LOL\).

Initially the goals were as much about learning distributed message based systems, as well as building something both of their companies could use. The first commit was pushed to GoogleCode on 12/26/2007, and shortly there after both Dru and Chris went to production with MassTransit and both of their companies have had success in getting value out of their efforts.

After four years of continued success, Chris and Dru continued to push forward on their Journey, and were joined by Travis Smith \(@TravisTheTechie\). The near future should bring much for the MassTransit community as RabbitMQ became the broker of choice, lessening the focus on MSMQ.

In early 2014, after a few years of research and design, work was started on an entirely new MassTransit. In order to embrace the world of asynchronous programming, as well as leveraging the power of advanced messaging platforms like RabbitMQ, a foundational rewrite was required. Much of the code in MassTransit was written prior to the introduction of the Task Parallel Library \(or TPL\), and even the .NET 4.0 support was before async and await were added to the language.

To eliminate a ton of extremely complex code, support for MSMQ was completely ripped out, including all of the routing support that had to be built because of MSMQ’s lack of message routing. The remaining code was rewritten from bottom to top, resulting in an entirely new, completely asynchronous, and highly optimized framework for message processing.

## The philosophy

First and foremost, we are not an Enterprise Service Bus \(ESB\). While MassTransit is used in several enterprises it isn’t a swiss army knife, we are not driven by sales to be a million features wide, and an inch deep. We focus on a few key concepts and try to make them as robust as possible.

We don’t do doodleware, you won’t find a designer, we are all about the keyboard samurais, the true in-the-trenches coder. That’s who we are, and those are our friends. If you want to draw, use a whiteboard.

We don’t do FTP-&gt;WS-deathstar-&gt;BS \(not that you can’t, it just isn't in the box\). We focus on the experience of using one transport in a given environment, and we try to make it as smooth as possible.

MassTransit is built to be used inside the firewall, it isn't built to be used as a means to communicate with external vendors \(it can be, again it just isn't in the box\), it's meant to be used for getting your corporate services talking to each other and making building internal software easier.
