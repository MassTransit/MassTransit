#r @"src/packages/FAKE/tools/FakeLib.dll"
open System.IO
open Fake
open Fake.AssemblyInfoFile
open Fake.Git.Information
open Fake.SemVerHelper
open System

let buildArtifactPath = FullName "./build_artifacts"
let packagesPath = FullName "./src/packages"
let keyFile = FullName "./MassTransit.snk"

let assemblyVersion = "4.0.0.0"
let baseVersion = "4.0.0"

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

let versionArgs = [ @"/p:Version=""" + NuGetVersion + @""""; @"/p:PackageVersion=""" + NuGetVersion + @""""; @"/p:AssemblyVersion=""" + FileVersion + @""""; @"/p:FileVersion=""" + FileVersion + @""""; @"/p:InformationalVersion=""" + InfoVersion + @"""" ]

printfn "Using version: %s" Version

Target "Clean" (fun _ ->
  ensureDirectory buildArtifactPath

  CleanDir buildArtifactPath
)

Target "RestorePackages" (fun _ -> 
  DotNetCli.Restore (fun p -> { p with Project = "./src/"
                                       AdditionalArgs = versionArgs })
)

Target "Build" (fun _ ->
  DotNetCli.Build (fun p-> { p with Project = @".\src\MassTransit.sln"
                                    Configuration= "Release"
                                    AdditionalArgs = versionArgs })
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


Target "Package" (fun _ ->
  DotNetCli.Pack (fun p-> { p with 
                                Project = @".\src\MassTransit.sln"
                                Configuration= "Release"
                                OutputPath= buildArtifactPath
                                AdditionalArgs = versionArgs @ [ @"--include-symbols"; @"--include-source" ] })
)

Target "Default" (fun _ ->
  trace "Build starting..."
)

"Clean"
  ==> "RestorePackages"
  ==> "Build"
  ==> "Package"
  ==> "Default"

RunTargetOrDefault "Default"