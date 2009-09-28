To execute NDepend during you MSBuild build process you can use the MSBuild Exec task like this:

<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <Target Name="ndepend"  >
    <Exec  Command="[PathWhereNDependExesAreStored]\NDepend.Console.exe [PathWhereNDependProjectFileIsStored]\[NDependProjectFileName].xml"/>
  </Target>
</Project>





You can also use the MSBuild task NDependTask stored in the assembly NDepend.Build.MSBuild.dll:

<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
   <UsingTask AssemblyFile="[PathWhereNDependExesAreStored]\MSBuild\NDepend.Build.MSBuild.dll" TaskName="NDependTask" />
   <Target Name="ndepend"  >
      <NDependTask NDependConsoleExePath="[PathWhereNDependExesAreStored]"
	           ProjectFilePath="[PathWhereNDependProjectFileIsStored]\[NDependProjectFileName].xml" /> 
   </Target>
</Project>