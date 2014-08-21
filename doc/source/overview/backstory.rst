What is MassTransit?
====================

MassTransit (MT) is a framework for creating distributed applications on
the .Net platform. MT provides the ability to subscribe to messages by type
and then connect different processing nodes through message subscriptions
building a cohesive mesh of services.

A bit of the back story?
------------------------

We are often asked why MassTransit was created, well here's the story. :)

In 2007, Chris Patterson (@phatboyg) and Dru Sellers (@drusellers)
met, for what Dru thinks is the first time, at the first Alt.Net conference.
It was at this conference that Chris and Dru not only realized that they had
a lot of the same problems that they needed to solve, but also how much the
standard tooling being provided by Microsoft just didn't fit their needs.
Surrounded by the best and brightest in .Net the energy to build better
tooling that supported testable processes, was aware of the latest advances
in tooling, libraries, and coding practices; they decided that a better
option must exist. After searching the .Net ecosystem for a tool that
would help them achieve goals, the only real option was the venerable
NServiceBus (NSB). After reviewing NSB, it was determined that the only
real IoC support was for Spring.Net, and that the NSB wasn't quite ready
for external contributors to come in. Therefore, they decided to embark
on the quixotic trek of building their own service bus.

Initially the goals were as much about learning about distributed
message based systems, as well as building something both of their
companies could use. The first commit was pushed to GoogleCode on
12/26/2007, and shortly there after both Dru and Chris went to
production with MT and both of their companies have had success in
getting value out of their efforts.

Now 4 years later, Chris and Dru continue to push forward
on their Journey, and have been joined by Travis Smith (@legomaster).
The near future should bring much for the MT community as RabbitMQ and
ActiveMQ support take front center with the quest for Mono support becoming
increasingly important.

The Philosophy
--------------

First and foremost, we are not an Enterprise Service Bus (ESB).
While MT is used in several enterprises its not a swiss army knife,
we are not driven by sales to be a million features wide, and an inch
deep. We focus on a few key concepts and try to make them as robust
as possible.

We don't do doodleware, you won't find a designer, we are all about
the keyboard samurais, the true in-the-trenches coder. That's who we are,
and those are our friends. If you want to draw, use a whiteboard.

We don't do FTP->WS-deathstar->BS (not that you can't, its just not
in the box). We focus on the experience of using one transport in a
given environment, and we try to make it as smooth as possible.

MT is built to be used inside the firewall, its not built to be used
as a means to communicate with external vendors (it can be, again its
just not in the box), its meant to be used for getting your corporate
services talking to each other and making building internal software
easier.
