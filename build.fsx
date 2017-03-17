#r @"src/packages/FAKE/tools/FakeLib.dll"
open System.IO
open Fake
open Fake.AssemblyInfoFile
open Fake.Git.Information
open Fake.SemVerHelper
open System

let buildOutputPath = "./build_output"
let buildArtifactPath = "./build_artifacts"
let nugetWorkingPath = FullName "./build_temp"
let packagesPath = FullName "./src/packages"
let keyFile = FullName "./MassTransit.snk"

let assemblyVersion = "3.5.0.0"
let baseVersion = "3.6.0"

let envVersion = (environVarOrDefault "APPVEYOR_BUILD_VERSION" (baseVersion + ".0"))
let buildVersion = (envVersion.Substring(0, envVersion.LastIndexOf('.')))

let semVersion : SemVerInfo = (parse buildVersion)

let Version = semVersion.ToString()

let branch = (fun _ ->
  (environVarOrDefault "APPVEYOR_REPO_BRANCH" (getBranchName "."))
)

let FileVersion = (environVarOrDefault "APPVEYOR_BUILD_VERSION" (Version + "." + "0"))

let informationalVersion = (fun _ ->
  let branchName = (branch ".")
  let label = if branchName="master" then "" else " (" + branchName + "/" + (getCurrentSHA1 ".").[0..7] + ")"
  (FileVersion + label)
)

let nugetVersion = (fun _ ->
  let branchName = (branch ".")
  let label = if branchName="master" then "" else "-" + (if branchName="mt3" then "beta" else branchName)
  let version = if branchName="master" then Version else FileVersion
  (version + label)
)

let InfoVersion = informationalVersion()
let NuGetVersion = nugetVersion()


printfn "Using version: %s" Version

Target "Clean" (fun _ ->
  ensureDirectory buildOutputPath
  ensureDirectory buildArtifactPath
  ensureDirectory nugetWorkingPath

  CleanDir buildOutputPath
  CleanDir buildArtifactPath
  CleanDir nugetWorkingPath
)

Target "RestorePackages" (fun _ -> 
     "./src/MassTransit.sln"
     |> RestoreMSSolutionPackages (fun p ->
         { p with
             OutputPath = packagesPath
             Retries = 4 })
)

Target "Build" (fun _ ->

  CreateCSharpAssemblyInfo @".\src\SolutionVersion.cs"
    [ Attribute.Title "MassTransit"
      Attribute.Description "MassTransit is a distributed application framework for .NET http://masstransit-project.com"
      Attribute.Product "MassTransit"
      Attribute.Version assemblyVersion
      Attribute.FileVersion FileVersion
      Attribute.InformationalVersion InfoVersion
    ]

  let buildMode = getBuildParamOrDefault "buildMode" "Release"
  let setParams defaults = { 
    defaults with
        Verbosity = Some(Quiet)
        Targets = ["Clean"; "Build"]
        Properties =
            [
                "Optimize", "True"
                "DebugSymbols", "True"
                "RestorePackages", "True"
                "Configuration", buildMode
                "Platform", "Any CPU"
            ]
  }

  build setParams @".\src\MassTransit.sln"
      |> DoNothing
)

let gitLink = (packagesPath @@ "gitlink" @@ "lib" @@ "net45" @@ "GitLink.exe")

Target "GitLink" (fun _ ->

    if String.IsNullOrWhiteSpace(gitLink) then failwith "Couldn't find GitLink.exe in the packages folder"

    let ok =
        execProcess (fun info ->
            info.FileName <- gitLink
            info.Arguments <- (sprintf "%s -u https://github.com/MassTransit/MassTransit" __SOURCE_DIRECTORY__)) (TimeSpan.FromSeconds 30.0)
    if not ok then failwith (sprintf "GitLink.exe %s' task failed" __SOURCE_DIRECTORY__)

)


let testDlls = [ "./src/MassTransit.Tests/bin/Release/MassTransit.Tests.dll"
                 "./src/MassTransit.AutomatonymousIntegration.Tests/bin/Release/MassTransit.AutomatonymousIntegration.Tests.dll" ]

Target "UnitTests" (fun _ ->
    testDlls
        |> NUnit (fun p -> 
            {p with
                Framework = "v4.0.30319"
                DisableShadowCopy = true; 
                OutputFile = buildArtifactPath + "/nunit-test-results.xml"})
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
                  Files = [ (@"..\src\MassTransit\bin\Release\MassTransit.*", Some @"lib\net452", None);
                            (@"..\src\MassTransit\**\*.cs", Some "src", None) ] }
                { Project = "MassTransit.Host"
                  Summary = "MassTransit Host Service"
                  PackageFile = @".\src\MassTransit.Host\packages.config"
                  Files = [ (@"..\src\MassTransit.Host\bin\Release\MassTransit.Host.*", Some @"lib\net452", None);
                            (@"..\host\*.*", Some @"tools", None);
                            (@"..\src\MassTransit.Host\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.RabbitMQ"
                  Summary = "MassTransit RabbitMQ Transport"
                  PackageFile = @".\src\MassTransit.RabbitMQTransport\packages.config"
                  Files = [ (@"..\src\MassTransit.RabbitMQTransport\bin\Release\MassTransit.RabbitMQTransport.*", Some @"lib\net452", None);
                            (@"..\src\MassTransit.RabbitMQTransport\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.AzureServiceBus"
                  Summary = "MassTransit Azure Service Bus Transport"
                  PackageFile = @".\src\MassTransit.AzureServiceBusTransport\packages.config"
                  Files = [ (@"..\src\MassTransit.AzureServiceBusTransport\bin\Release\MassTransit.AzureServiceBusTransport.*", Some @"lib\net452", None);
                            (@"..\src\MassTransit.AzureServiceBusTransport\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.Http"
                  Summary = "MassTransit HTTP Transport"
                  PackageFile = @".\src\MassTransit.HttpTransport\packages.config"
                  Files = [ (@"..\src\MassTransit.HttpTransport\bin\Release\MassTransit.HttpTransport.*", Some @"lib\net452", None);
                            (@"..\src\MassTransit.HttpTransport\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.Automatonymous"
                  Summary = "MassTransit Automatonymous State Machine Support"
                  PackageFile = @".\src\MassTransit.AutomatonymousIntegration\packages.config"
                  Files = [ (@"..\src\MassTransit.AutomatonymousIntegration\bin\Release\MassTransit.AutomatonymousIntegration.*", Some @"lib\net452", None);
                            (@"..\src\MassTransit.AutomatonymousIntegration\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.Log4Net"
                  Summary = "MassTransit Log4Net Logging Support"
                  PackageFile = @".\src\Loggers\MassTransit.Log4NetIntegration\packages.config"
                  Files = [ (@"..\src\Loggers\MassTransit.Log4NetIntegration\bin\Release\MassTransit.Log4NetIntegration.*", Some @"lib\net452", None);
                            (@"..\src\Loggers\MassTransit.Log4NetIntegration\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.NLog"
                  Summary = "MassTransit NLog Logging Support"
                  PackageFile = @".\src\Loggers\MassTransit.NLogIntegration\packages.config"
                  Files = [ (@"..\src\Loggers\MassTransit.NLogIntegration\bin\Release\MassTransit.NLogIntegration.*", Some @"lib\net452", None);
                            (@"..\src\Loggers\MassTransit.NLogIntegration\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.SerilogIntegration"
                  Summary = "MassTransit Serilog Logging Support"
                  PackageFile = @".\src\Loggers\MassTransit.SerilogIntegration\packages.config"
                  Files = [ (@"..\src\Loggers\MassTransit.SerilogIntegration\bin\Release\MassTransit.SerilogIntegration.*", Some @"lib\net452", None);
                            (@"..\src\Loggers\MassTransit.SerilogIntegration\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.Autofac"
                  Summary = "MassTransit Autofac Container Support"
                  PackageFile = @".\src\Containers\MassTransit.AutofacIntegration\packages.config"
                  Files = [ (@"..\src\Containers\MassTransit.AutofacIntegration\bin\Release\MassTransit.AutofacIntegration.*", Some @"lib\net452", None);
                            (@"..\src\Containers\MassTransit.AutofacIntegration\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.Automatonymous.Autofac"
                  Summary = "MassTransit Automatonymous Autofac Container Support"
                  PackageFile = @".\src\Containers\MassTransit.Automatonymous.AutofacIntegration\packages.config"
                  Files = [ (@"..\src\Containers\MassTransit.Automatonymous.AutofacIntegration\bin\Release\MassTransit.Automatonymous.AutofacIntegration.*", Some @"lib\net452", None);
                            (@"..\src\Containers\MassTransit.Automatonymous.AutofacIntegration\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.CastleWindsor"
                  Summary = "MassTransit Castle Windsor Container Support"
                  PackageFile = @".\src\Containers\MassTransit.WindsorIntegration\packages.config"
                  Files = [ (@"..\src\Containers\MassTransit.WindsorIntegration\bin\Release\MassTransit.WindsorIntegration.*", Some @"lib\net452", None);
                            (@"..\src\Containers\MassTransit.WindsorIntegration\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.SimpleInjector"
                  Summary = "MassTransit SimpleInjector Container Support"
                  PackageFile = @".\src\Containers\MassTransit.SimpleInjectorIntegration\packages.config"
                  Files = [ (@"..\src\Containers\MassTransit.SimpleInjectorIntegration\bin\Release\MassTransit.SimpleInjectorIntegration.*", Some @"lib\net452", None);
                            (@"..\src\Containers\MassTransit.SimpleInjectorIntegration\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.StructureMap"
                  Summary = "MassTransit StructureMap Container Support"
                  PackageFile = @".\src\Containers\MassTransit.StructureMapIntegration\packages.config"
                  Files = [ (@"..\src\Containers\MassTransit.StructureMapIntegration\bin\Release\MassTransit.StructureMapIntegration.*", Some @"lib\net452", None);
                            (@"..\src\Containers\MassTransit.StructureMapIntegration\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.Automatonymous.StructureMap"
                  Summary = "MassTransit Automatonymous StructureMap Container Support"
                  PackageFile = @".\src\Containers\MassTransit.Automatonymous.StructureMapIntegration\packages.config"
                  Files = [ (@"..\src\Containers\MassTransit.Automatonymous.StructureMapIntegration\bin\Release\MassTransit.Automatonymous.StructureMapIntegration.*", Some @"lib\net452", None);
                            (@"..\src\Containers\MassTransit.Automatonymous.StructureMapIntegration\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.Unity"
                  Summary = "MassTransit Unity Container Support"
                  PackageFile = @".\src\Containers\MassTransit.UnityIntegration\packages.config"
                  Files = [ (@"..\src\Containers\MassTransit.UnityIntegration\bin\Release\MassTransit.UnityIntegration.*", Some @"lib\net452", None);
                            (@"..\src\Containers\MassTransit.UnityIntegration\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.Ninject"
                  Summary = "MassTransit Ninject Container Support"
                  PackageFile = @".\src\Containers\MassTransit.NinjectIntegration\packages.config"
                  Files = [ (@"..\src\Containers\MassTransit.NinjectIntegration\bin\Release\MassTransit.NinjectIntegration.*", Some @"lib\net452", None);
                            (@"..\src\Containers\MassTransit.NinjectIntegration\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.NHibernate"
                  Summary = "MassTransit NHibernate Saga Storage"
                  PackageFile = @".\src\Persistence\MassTransit.NHibernateIntegration\packages.config"
                  Files = [ (@"..\src\Persistence\MassTransit.NHibernateIntegration\bin\Release\MassTransit.NHibernateIntegration.*", Some @"lib\net452", None);
                            (@"..\src\Persistence\MassTransit.NHibernateIntegration\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.EntityFramework"
                  Summary = "MassTransit Entity Framework Saga Storage"
                  PackageFile = @".\src\Persistence\MassTransit.EntityFrameworkIntegration\packages.config"
                  Files = [ (@"..\src\Persistence\MassTransit.EntityFrameworkIntegration\bin\Release\MassTransit.EntityFrameworkIntegration.*", Some @"lib\net452", None);
                            (@"..\src\Persistence\MassTransit.EntityFrameworkIntegration\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.MongoDb"
                  Summary = "MassTransit MongoDb Saga and Message Data Storage"
                  PackageFile = @".\src\Persistence\MassTransit.MongoDbIntegration\packages.config"
                  Files = [ (@"..\src\Persistence\MassTransit.MongoDbIntegration\bin\Release\MassTransit.MongoDbIntegration.*", Some @"lib\net452", None);
                            (@"..\src\Persistence\MassTransit.MongoDbIntegration\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.Redis"
                  Summary = "MassTransit Redis Saga Storage"
                  PackageFile = @".\src\Persistence\MassTransit.RedisIntegration\packages.config"
                  Files = [ (@"..\src\Persistence\MassTransit.RedisIntegration\bin\Release\MassTransit.RedisIntegration.*", Some @"lib\net452", None);
                            (@"..\src\Persistence\MassTransit.RedisIntegration\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.Marten"
                  Summary = "MassTransit Marten (Postgresql JSONB) Saga Storage"
                  PackageFile = @".\src\Persistence\MassTransit.MartenIntegration\packages.config"
                  Files = [ (@"..\src\Persistence\MassTransit.MartenIntegration\bin\Release\MassTransit.MartenIntegration.*", Some @"lib\net461", None);
                            (@"..\src\Persistence\MassTransit.MartenIntegration\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.TestFramework"
                  Summary = "MassTransit NUnit Test Framework"
                  PackageFile = @".\src\MassTransit.TestFramework\packages.config"
                  Files = [ (@"..\src\MassTransit.TestFramework\bin\Release\MassTransit.TestFramework.*", Some @"lib\net452", None);
                            (@"..\src\MassTransit.TestFramework\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.Reactive"
                  Summary = "MassTransit Reactive Extensions Support"
                  PackageFile = @".\src\MassTransit.Reactive\packages.config"
                  Files = [ (@"..\src\MassTransit.Reactive\bin\Release\MassTransit.Reactive.*", Some @"lib\net452", None);
                            (@"..\src\MassTransit.Reactive\**\*.cs", Some @"src", None) ] } 
                { Project = "MassTransit.Quartz"
                  Summary = "MassTransit Quartz Scheduler Support"
                  PackageFile = @".\src\MassTransit.QuartzIntegration\packages.config"
                  Files = [ (@"..\src\MassTransit.QuartzIntegration\bin\Release\MassTransit.QuartzIntegration.*", Some @"lib\net452", None);
                            (@"..\src\MassTransit.QuartzIntegration\**\*.cs", Some @"src", None) ] } 
             |]

  nugs
    |> Array.iter (fun nug ->

      let getDeps daNug : NugetDependencies =
        if daNug.Project = "MassTransit" then (getDependencies daNug.PackageFile)
        else if daNug.Project = "MassTransit.Host" then (("MassTransit", NuGetVersion) :: ("MassTransit.Autofac", NuGetVersion) :: ("MassTransit.Log4Net", NuGetVersion) :: (getDependencies daNug.PackageFile))
        else ("MassTransit", NuGetVersion) :: (getDependencies daNug.PackageFile)

      let setParams defaults = {
        defaults with 
          Authors = ["Chris Patterson"; "Dru Sellers"; "Travis Smith" ]
          Description = "MassTransit is a distributed application framework for .NET, including support for RabbitMQ and Azure Service Bus."
          OutputPath = buildArtifactPath
          Project = nug.Project
          Dependencies = (getDeps nug)
          Summary = nug.Summary
          SymbolPackage = NugetSymbolPackage.Nuspec
          Version = NuGetVersion
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
  ==> "RestorePackages"
  ==> "Build"
  ==> "GitLink"
  ==> "Package"
  ==> "Default"

RunTargetOrDefault "Default"