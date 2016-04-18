// --------------------------------------------------------------------------------------
// FAKE build script
// --------------------------------------------------------------------------------------
module Build

#r "packages/FAKE/tools/FakeLib.dll"

open Fake
open Fake.Testing
open System
open Fake.Git
open System.IO

let buildDir = ".build/"
let testDir = ".test/"
let srcPath = !!"**/*.fsproj"

Target "Default" (fun _ -> trace "Hello World from FAKE")
Target "Clean" (fun _ -> 
    trace "Clean build dirs"
    CleanDirs [ buildDir; testDir ])
Target "BuildApp" (fun _ -> 
    trace "building app"
    srcPath
    |> MSBuildDebug buildDir "Build"
    |> Log "AppBuildDebug-Output: ")
"Clean" ==> "BuildApp" ==> "Default"
RunTargetOrDefault "Default"
