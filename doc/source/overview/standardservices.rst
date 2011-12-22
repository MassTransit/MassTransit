Standard Services
====================================

.. note::

    needs to be cleaned up

Installing MSMQ

run the install_msmq.bat
run the server_install_msmq.bat
You can also do this form C# code when configuring the bus.
Subscription Service

- Installation

MSMQ should be installed (see Installing MSMQ)
By default this service is set to run in-memory store
Run the create_[servicename]_queue.vbs
Go to the queues Properties>Security tab and ensure to give ‘local administrators’ Full Access. By default MSMQ only gives YOU access.  Leave all other permissions at default.
Copy the files wherever you want them
run ‘SubscriptionServiceHost.exe /install’
Health Service

- Installation

MSMQ should be installed (see Installing MSMQ)
By default this service is set to run in-memory store
Run the create_[servicename]_queue.vbs
Go to the queues Properties>Security tab and ensure to give ‘local administrators’ Full Access. By default MSMQ only gives YOU access.  Leave all other permissions at default.
Copy the files wherever you want them
run ‘SubscriptionServiceHost.exe /install’
Timeout Service

- Installation

MSMQ should be installed (see Installing MSMQ)
By default this service is set to run in-memory store
Run the create_[servicename]_queue.vbs
Go to the queues Properties>Security tab and ensure to give ‘local administrators’ Full Access. By default MSMQ only gives YOU access.  Leave all other permissions at default.
Copy the files wherever you want them
run ‘SubscriptionServiceHost.exe /install’
Health, and Subscription Services can be as one ‘Runtime’ services instead of distinct services

MSMQ should be installed
The service will attempt to create any missing queues
Copy the files to where you want them
Edit the MassTransit.RuntimeServices.exe.config to point to the right connection string and subscriptions queue
run ‘MassTransit.RuntimeServices.exe /install’
To set these services up to run with a persistant (MSSSQL) store

A SQL Server 2008 database installed. If it is not on the local drive, please follow the setup considerations below.
Mass Transit will try to install to your local database. If you need it to install to a different one, please edit the ”SET INSTANCE=(local)” in the _install.cmd file and set that to a database instance you would like to use. Ensure you have permission to create databases on that instance.
Open the servicename.castle.xml in an editor.  Find the hibernate.connection.connection_string in the facilities section of the file.  Edit the server location.
If you would like to set a more secure password for the login, please update it in the SQLScripts\CreateUserAccountLoginDDL.sql and also in the hibernate connection string from the step before this.
