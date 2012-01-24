# Welcome to the albacore project.

albacore is a suite of Rake tasks to automate the process of building a .NET based system. It's like MSBuild or Nant... but without all the stabby-bracket xmlhell.

## How To Use albacore

Check out the [albacore Wiki](http://wiki.github.com/derickbailey/albacore) for detailed instructions on how to use the built in tasks and their options. 

If you are new to Ruby and Rake, head over to the [getting started](https://github.com/derickbailey/Albacore/wiki/Getting-Started) wiki page.

## Supported Versions Of Ruby And Rake

Albacore has been tested against the following versions of rake:

* Rake v0.8.7
* Rake v0.9.2

Albacore has been tested against the following versions of Ruby for Windows:

* RubyInstaller v1.8.7
* RubyInstaller v1.9.2
* IronRuby v1.0
* IronRuby v1.1
* IronRuby v1.1.1
* IronRuby v1.1.2
* IronRuby v1.1.3

### Unsupported Versions Of Ruby

Support for the following versions of ruby has been dropped. Albacore will no longer be tested against, or have code written to work with these versions of ruby. Use these versions at your own risk.

* RubyInstaller v1.8.6
* RubyInstaller v1.9.1

### Notes About IronRuby

Due to an incompatibility with the Rubyzip gem, IronRuby does not support the ‘zip’ and ‘unzip’ tasks. If you need zip / unzip support, look into using a third party tool such as [7-zip](http://7-zip.org) or [SharpZipLib](http://sharpdevelop.net/OpenSource/SharpZipLib/).

## Legal Mumbo Jumbo (MIT License)

Copyright (c) 2011 Derick Bailey

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
