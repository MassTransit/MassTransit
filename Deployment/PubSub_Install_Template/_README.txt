SETUP REQUIREMENTS:
1. MSMQ installed on the machine where MT will be installed.
2. A SQL Server 2005 database installed. If it is not on the local drive, please follow the setup considerations below.


SETUP CONSIDERATIONS:
1. Mass Transit will try to install to your local database. If you need it to install to a different one, please edit the 
"SET INSTANCE=(local)" in the _install.cmd file and set that to a database instance you would like to use. Ensure you have permission to create databases on that instance.  Do the same for the _uninstall.cmd.
2. Open the SubscriptionManager.xml in an editor.  Find the hibernate.connection.connection_string in the facilities section of the file.  Please edit the server location.
3. If you would like to set a more secure password for the login, please update it in the SQLScripts\CreateUserAccountLoginDDL.sql and also in the hibernate connection string from the step before this.


INSTALL:
1. The location for the physical database may need to be changed.  This is located in SQLScripts\CreateMassTransitDatabaseDDL.sql. Check the file for instructions.
2. Create a new private msmq named 'mt_pubsub'. Go to its Properties>Security tab and ensure to give all local administrators Full access to ‘Everyone’.  Leave all other permissions at default.
2. Please run the _install.cmd.


UNINSTALL:
1. Please run the _uninstall.cmd.
2. Remove the Private MSMQ named "mt_pubsub."