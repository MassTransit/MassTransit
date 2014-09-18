How to install
""""""""""""""

NuGet
'''''

The simplest way to install MassTransit into your solution/project is to use
NuGet.::

    nuget Install-Package MassTransit

However, the NuGet packages don't contain the MassTransit.RuntimeServices executable and database SQL scripts.  The RuntimeServices system routes messages to multiple subscribers via the Subscription Service.  If you plan to use the "UseSubscriptionService" feature, then you'll need to get compile that from source.


Compiling From Source
'''''''''''''''''''''

Lastly, if you want to hack on MassTransit or just want to have the actual source
code you can clone the source from github.com.

To clone the repository using git try the following::

    git clone git://github.com/MassTransit/MassTransit.git

If you want the development branch (where active development happens)::

    git clone git://github.com/MassTransit/MassTransit.git
    git checkout develop

Build Dependencies
''''''''''''''''''

To compile MassTransit from source you will need the following developer tools
installed:

 * .Net 4.0 sdk
 * ruby v 1.8.7
 * gems (rake, albacore)

Compiling
'''''''''

To compile the source code, drop to the command line and type::

    .\build.bat

If you look in the ``.\build_output`` folder you should see the binaries.
