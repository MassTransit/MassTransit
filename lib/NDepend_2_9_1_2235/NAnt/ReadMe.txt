To execute NDepend during you NAnt build process you can use the NAnt exec task like this:

<?xml version="1.0" ?>
<project name="nant" default="compile" xmlns="http://nant.sf.net/schemas/nant.xsd">
  <target name="ndepend">
    <exec program="[PathWhereNDependExesAreStored]\NDepend.Console.exe">
      <arg value="[PathWhereNDependProjectFileIsStored]\[NDependProjectFileName].xml" />
    </exec>
  </target>
</project>




When NAnt will be able to use tasks compiled with .NET 2.0.50727, you'll be able to use the NDepend task like this (don't forget to first copy the assembly NDepend.Build.NAntTasks.dll into your NAnt dir):

<?xml version="1.0" ?>
<project name="nant" default="compile" xmlns="http://nant.sf.net/schemas/nant.xsd">
  <target name="ndepend">
    <NDepend NDependConsoleExePath="[PathWhereNDependExesAreStored]" 
                 ProjectFilePath="[PathWhereNDependProjectFileIsStored]\[NDependProjectFileName].xml"/>
  </target>
</project>

