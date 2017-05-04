# Testing

Testing is a complex subject in asynchronous, distributed systems. 
MassTransit offers some support for developers that wish to test how messages are being consumed,
how messages get sent and published, how requests are handled and responses are produced.

Core MassTransit provides a few test harnesses for different types of tests, which are
test framework agnostic and do some complex wiring inside them, delivering a comfortable
testing API to developers.

Refer to the following pages to know more how test your services:

 * [Bus harness](bus-harness.md)
 * [Testing consumers](testing-consumers.md)
 * [Testing sagas](testing-sagas.md)
