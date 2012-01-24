Mass Transit - A Service Bus Implementation for .NET
=======

![Mass Transit](http://www.phatboyg.com/mt-logo.png "Mass Transit")

# LICENSE
Apache 2.0 - see LICENSE

# IMPORTANT
NOTE: If you are looking at the source - please run build.bat before opening the solution. It creates the SolutionVersion.cs file that is necessary for a successful build.

# INFO
## Overview
MassTransit is lean service bus implementation for building loosely coupled applications using the .NET framework.

## Getting started with Mass Transit
### Documentation

Documentation is located at [http://docs.masstransit-project.com/](http://docs.masstransit-project.com/).

### Downloads
 Download straight from NuGet 'MassTransit' [Search NuGet for MassTransit](http://nuget.org/packages?q=masstransit)
 
 Download officially released builds from 
 [Github](http://github.com/masstransit/masstransit/downloads/).
 
 Download Nightly Binaries from [TeamCity](http://teamcity.codebetter.com/viewType.html?buildTypeId=bt8&tab=buildTypeStatusDiv).

### Mailing List

[MassTransit Discuss](http://groups.google.com/group/masstransit-discuss)

### Source

1. Clone the source down to your machine. 
  `git clone git://github.com/MassTransit/MassTransit.git`
2. Ensure Ruby is installed. [RubyInstaller for Windows](http://rubyinstaller.org/)
3. Ensure Bundler is installed.
  `gem install bundler`
   
    * Then do: `bundle install` which will install all dependencies.

4. Run `build.bat`.

### Contributing 

1. `git config --global core.autoclrf false`
2. Shared ReSharper settings are under src/MassTransit.resharper.xml
3. Make a pull request

`bundle exec rake global_version` generates the global version if you prefer building with MsBuild/Visual Studio. 
 
 
# REQUIREMENTS
* .NET Framework 3.5


# CREDITS
Logo Design by [The Agile Badger](http://www.theagilebadger.com)  
UppercuT - Automated Builds in moments, not days! [Project UppercuT](http://projectuppercut.org)
