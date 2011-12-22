Logging in MassTransit
======================

Logging in MassTransit is done with the de facto logging tool 'log4net.' This tool was chosen for its many years of battle hardened code and ease of use. You can find out more about log4net and its configuration at http://logging.apache.org/log4net

Like NHibernate's 'NHibernate.SQL' where all of NHibernate's generated sql is logged, MassTransit has a log named 'MassTransit.Messages' where all of the message traffic is logged. This logging looks like:

    RECV:{Address}:{Message Id}:{Message Type Name}
    SEND:{Address}:{Message Name}