#r @"src/packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.AssemblyInfoFile
open Fake.Git.Information

let buildOutputPath = "./build_output"
let buildArtifactPath = "./build_artifacts"
let keyFile = FullName "./MassTransit.snk"

let Version = "3.0.11-beta"

Target "Clean" (fun _ ->
  CleanDir buildOutputPath
  CleanDir buildArtifactPath
)

Target "Build" (fun _ ->

  CreateCSharpAssemblyInfo "./src/SolutionVersion.cs"
    [ Attribute.Title "MassTransit"
      Attribute.Description "MassTransit is a distributed application framework for .NET http://masstransit-project.com"
      Attribute.Product "MassTransit"
      Attribute.Version Version
      Attribute.FileVersion Version ]

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

  build setParams "./src/MassTransit.sln"
      |> DoNothing
)

Target "CopyOutput" (fun _ ->
    Copy (buildOutputPath @@ "net-4.5") !! ("./src/MassTransit/bin/Release/MassTransit.*")
)



Target "Default" (fun _ ->
  trace "Build starting..."
)

"Clean"
  ==> "Build"
  ==> "CopyOutput"
  ==> "Default"

RunTargetOrDefault "Default"