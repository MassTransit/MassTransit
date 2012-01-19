Sharp Tests Extensions (#TestsEx for friends)
=============================================


How to build:
-------------

Run build.bat the result of compilation will be available in
.\Build\Output.


Which assembly do I need to use #TestsEx?
-----------------------------------------

You need only the assemblies deployed depending on the unit
tests framework you are using.

For NUnit you need ONLY assemblies contained in NUnit folder

For xUnit you need ONLY assemblies contained in xUnit folder

For Silverlight you need ONLY assemblies contained in Silverlight folder

For MsTest you need ONLY assemblies contained in MsTest folder(read http://sharptestex.codeplex.com/wikipage?title=HowToV1VS2010 )


When do I need the NoSpecificFramework ?
----------------------------------------

If the unit tests framework you are using is not directly supported,
you can use the assemblies contained in NoSpecificFramework folder.
The main difference will be that your runner will not recognize 
the exception and its output will look slightly less "pretty" than native assertions.

