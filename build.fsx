#r @"packages/FAKE/tools/FakeLib.dll"
#load "build-helpers.fsx"

open Fake
open System
open System.IO
open System.Linq
open BuildHelpers
open Fake.XamarinHelper


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



"common-build"
    ==> "android-package"


RunTargetOrDefault "android-package"