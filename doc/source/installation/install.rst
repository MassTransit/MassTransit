How to install
""""""""""""""

NuGet
'''''

The simplest way to install MassTransit into your solution/project is to use
NuGet.::

    nuget Install-Package MassTransit


Raw Binaries
''''''''''''

If you are a fan of getting the binaries you can get released builds from

http://github.com/masstransit/MassTransit/downloads


Compiling From Source
'''''''''''''''''''''

Lastly, if you want to hack on MassTransit or just want to have the actual source
you can do that my pulling the source from github.com


Then you will need to clone the github repository using ``git``:::

    git clone git://github.com/MassTransit/MassTransit.git

If you want the development branch::

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