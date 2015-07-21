#r @"src/packages/FAKE/tools/FakeLib.dll"
open System.IO
open Fake
open Fake.AssemblyInfoFile
open Fake.Git.Information
open Fake.SemVerHelper

let buildOutputPath = "./build_output"
let buildArtifactPath = "./build_artifacts"
let nugetWorkingPath = FullName "./build_temp"
let keyFile = FullName "./MassTransit.snk"

let baseVersion = "3.0.11"

let semVersion : SemVerInfo = parse baseVersion

let Version = semVersion.ToString()

let branch = (fun _ ->
  (environVarOrDefault "TEAMCITY_BUILD_BRANCH" (getBranchName "."))
)

let fileVersion = (Version + "." + (environVarOrDefault "BUILD_NUMBER" "0"))

let informationalVersion = (fun _ ->
  let branchName = (branch ".")
  let label = if branchName="master" then "" else " (" + branchName + "/" + (getCurrentSHA1 ".").[0..7] + ")"
  (fileVersion + label)
)

let nugetVersion = (fun _ ->
  let branchName = (branch ".")
  let label = if branchName="master" then "" else "-" + branchName
  (Version + label)
)

printfn "Using version: %s" Version

Target "Clean" (fun _ ->
  ensureDirectory buildOutputPath
  ensureDirectory buildArtifactPath
  ensureDirectory nugetWorkingPath

  CleanDir buildOutputPath
  CleanDir buildArtifactPath
  CleanDir nugetWorkingPath
)

Target "Build" (fun _ ->

  CreateCSharpAssemblyInfo @".\src\SolutionVersion.cs"
    [ Attribute.Title "MassTransit"
      Attribute.Description "MassTransit is a distributed application framework for .NET http://masstransit-project.com"
      Attribute.Product "MassTransit"
      Attribute.Version Version
      Attribute.FileVersion (fileVersion)
      Attribute.InformationalVersion (informationalVersion())
    ]

  let buildMode = getBuildParamOrDefault "buildMode" "Release"
  let setParams defaults = { 
    defaults with
        Verbosity = Some(Quiet)
        Targets = ["Rebuild"]
        Properties =
            [
                "Optimize", "True"
                "DebugSymbols", "True"
                "Configuration", buildMode
                "SignAssembly", "True"
                "AssemblyOriginatorKeyFile", keyFile
                "TargetFrameworkVersion", "v4.5"
                "Platform", "Any CPU"
            ]
  }

  build setParams @".\src\MassTransit.sln"
      |> DoNothing
)

type packageInfo = {
    Project: string
    PackageFile: string
    Summary: string
    Files: list<string*string option*string option>
}

Target "Package" (fun _ ->

  let nugs = [| { Project = "MassTransit"
                  Summary = "MassTransit, a mesage-based distributed application framework"
                  PackageFile = @".\src\MassTransit\packages.config"
                  Files = [ (@"..\src\MassTransit\bin\Release\MassTransit.*", Some @"lib\net45", None);
                            (@"..\src\MassTransit\**\*.cs", Some "src", None) ] }
                { Project = "MassTransit.RabbitMQ"
                  Summary = "MassTransit RabbitMQ Transport"
                  PackageFile = @".\src\MassTransit.RabbitMQTransport\packages.config"
                  Files = [ (@"..\src\MassTransit.RabbitMQTransport\bin\Release\MassTransit.RabbitMQTransport.*", Some @"lib\net45", None);
                            (@"..\src\MassTransit.RabbitMQTransport\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.AzureServiceBus"
                  Summary = "MassTransit Azure Service Bus Transport"
                  PackageFile = @".\src\MassTransit.AzureServiceBusTransport\packages.config"
                  Files = [ (@"..\src\MassTransit.AzureServiceBusTransport\bin\Release\MassTransit.AzureServiceBusTransport.*", Some @"lib\net45", None);
                            (@"..\src\MassTransit.AzureServiceBusTransport\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.Automatonymous"
                  Summary = "MassTransit Automatonymous State Machine Support"
                  PackageFile = @".\src\MassTransit.AutomatonymousIntegration\packages.config"
                  Files = [ (@"..\src\MassTransit.AutomatonymousIntegration\bin\Release\MassTransit.AutomatonymousIntegration.*", Some @"lib\net45", None);
                            (@"..\src\MassTransit.AutomatonymousIntegration\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.Log4Net"
                  Summary = "MassTransit Log4Net Logging Support"
                  PackageFile = @".\src\Loggers\MassTransit.Log4NetIntegration\packages.config"
                  Files = [ (@"..\src\Loggers\MassTransit.Log4NetIntegration\bin\Release\MassTransit.Log4NetIntegration.*", Some @"lib\net45", None);
                            (@"..\src\Loggers\MassTransit.Log4NetIntegration\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.NLog"
                  Summary = "MassTransit NLog Logging Support"
                  PackageFile = @".\src\Loggers\MassTransit.NLogIntegration\packages.config"
                  Files = [ (@"..\src\Loggers\MassTransit.NLogIntegration\bin\Release\MassTransit.NLogIntegration.*", Some @"lib\net45", None);
                            (@"..\src\Loggers\MassTransit.NLogIntegration\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.Autofac"
                  Summary = "MassTransit Autofac Container Support"
                  PackageFile = @".\src\Containers\MassTransit.AutofacIntegration\packages.config"
                  Files = [ (@"..\src\Containers\MassTransit.AutofacIntegration\bin\Release\MassTransit.AutofacIntegration.*", Some @"lib\net45", None);
                            (@"..\src\Containers\MassTransit.AutofacIntegration\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.CastleWindsor"
                  Summary = "MassTransit Castle Windsor Container Support"
                  PackageFile = @".\src\Containers\MassTransit.WindsorIntegration\packages.config"
                  Files = [ (@"..\src\Containers\MassTransit.WindsorIntegration\bin\Release\MassTransit.WindsorIntegration.*", Some @"lib\net45", None);
                            (@"..\src\Containers\MassTransit.WindsorIntegration\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.StructureMap"
                  Summary = "MassTransit StructureMap Container Support"
                  PackageFile = @".\src\Containers\MassTransit.StructureMapIntegration\packages.config"
                  Files = [ (@"..\src\Containers\MassTransit.StructureMapIntegration\bin\Release\MassTransit.StructureMapIntegration.*", Some @"lib\net45", None);
                            (@"..\src\Containers\MassTransit.StructureMapIntegration\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.Unity"
                  Summary = "MassTransit Unity Container Support"
                  PackageFile = @".\src\Containers\MassTransit.UnityIntegration\packages.config"
                  Files = [ (@"..\src\Containers\MassTransit.UnityIntegration\bin\Release\MassTransit.UnityIntegration.*", Some @"lib\net45", None);
                            (@"..\src\Containers\MassTransit.UnityIntegration\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.Ninject"
                  Summary = "MassTransit Ninject Container Support"
                  PackageFile = @".\src\Containers\MassTransit.NinjectIntegration\packages.config"
                  Files = [ (@"..\src\Containers\MassTransit.NinjectIntegration\bin\Release\MassTransit.NinjectIntegration.*", Some @"lib\net45", None);
                            (@"..\src\Containers\MassTransit.NinjectIntegration\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.NHibernate"
                  Summary = "MassTransit NHibernate Saga Transport"
                  PackageFile = @".\src\Persistence\MassTransit.NHibernateIntegration\packages.config"
                  Files = [ (@"..\src\Persistence\MassTransit.NHibernateIntegration\bin\Release\MassTransit.NHibernateIntegration.*", Some @"lib\net45", None);
                            (@"..\src\Persistence\MassTransit.NHibernateIntegration\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.TestFramework"
                  Summary = "MassTransit NUnit Test Framework"
                  PackageFile = @".\src\MassTransit.TestFramework\packages.config"
                  Files = [ (@"..\src\MassTransit.TestFramework\bin\Release\MassTransit.TestFramework.*", Some @"lib\net45", None);
                            (@"..\src\MassTransit.TestFramework\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.Reactive"
                  Summary = "MassTransit Reactive Extensions Support"
                  PackageFile = @".\src\MassTransit.Reactive\packages.config"
                  Files = [ (@"..\src\MassTransit.Reactive\bin\Release\MassTransit.Reactive.*", Some @"lib\net45", None);
                            (@"..\src\MassTransit.Reactive\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.Quartz"
                  Summary = "MassTransit Quartz Scheduler Support"
                  PackageFile = @".\src\MassTransit.QuartzIntegration\packages.config"
                  Files = [ (@"..\src\MassTransit.QuartzIntegration\bin\Release\MassTransit.QuartzIntegration.*", Some @"lib\net45", None);
                            (@"..\src\MassTransit.QuartzIntegration\**\*.cs", Some @"src", None) ] } 
             |]

  nugs
    |> Array.iter (fun nug ->

      let getDeps daNug : NugetDependencies =
        if daNug.Project = "MassTransit" then (getDependencies daNug.PackageFile)
        else ("MassTransit", (nugetVersion())) :: (getDependencies daNug.PackageFile)

      let setParams defaults = {
        defaults with 
          Authors = ["Chris Patterson"; "Dru Sellers"; "Travis Smith" ]
          Description = "MassTransit is a distributed application framework for .NET, including support for RabbitMQ and Azure Service Bus."
          OutputPath = buildArtifactPath
          Project = nug.Project
          Dependencies = (getDeps nug)
          Summary = nug.Summary
          SymbolPackage = NugetSymbolPackage.Nuspec
          Version = (nugetVersion())
          WorkingDir = nugetWorkingPath
          Files = nug.Files
      } 

      NuGet setParams (FullName "./template.nuspec")
    )
)

Target "Default" (fun _ ->
  trace "Build starting..."
)

"Clean"
  ==> "Build"
  ==> "Package"
  ==> "Default"

RunTargetOrDefault "Default"