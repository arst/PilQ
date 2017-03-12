#r @"packages/FAKE/tools/FakeLib.dll"
#load "build-helpers.fsx"
#load "HockeyAppHelper.fsx"

open Fake
open System
open System.IO
open System.Linq
open BuildHelpers
open Fake.XamarinHelper
open HockeyAppHelper


Target "common-build" (fun () ->
    RestorePackages "PilQ.sln"

    MSBuild "bin/Release" "Build" [ ("Configuration", "Release"); ("Platform", "Any CPU") ] [ "PilQ.sln" ] |> ignore
)

Target "android-package" (fun () ->
    AndroidPackage (fun defaults ->
        {defaults with
            ProjectPath = "PilQ.Android.csproj"
            Configuration = "Release"
            OutputPath = "bin/Release"
        }) 
    |> AndroidSignAndAlign (fun defaults ->
        {defaults with 
            KeystorePath = "pilq.keystore"
            KeystorePassword = Environment.GetEnvironmentVariable("PilQKeystorePassword")
            KeystoreAlias = "pilq"
            ZipalignPath = @"C:\Program Files (x86)\Android\android-sdk\build-tools\23.0.1\zipalign.exe"
        })
   |> fun file -> TeamCityHelper.PublishArtifact file.FullName
)

Target "android-hockeyapp" (fun () ->
    let buildCounter = BuildHelpers.GetBuildCounter TeamCityHelper.TeamCityBuildNumber

    let hockeyAppApiToken = Environment.GetEnvironmentVariable("HockeyAppApiToken")

    let appPath = Directory.EnumerateFiles(Path.Combine( "Todo.Android", "bin", "Release"), "*.apk", SearchOption.AllDirectories).First()

    HockeyAppHelper.Upload hockeyAppApiToken appPath buildCounter
)



"common-build"
    ==> "android-package"
    ==> "android-hockeyapp"


RunTargetOrDefault "android-package"