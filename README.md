# Installation
[![NuGet version](https://badge.fury.io/nu/Cake.SoftNUnit3.svg)](https://badge.fury.io/nu/Cake.SoftNUnit3)

Import **Cake.SoftNUnit3** NuGet package into your cake script for running NUnit3 tests without failing cake task if any test was failed.

# Usage example

```cake
#tool nuget:?package=NUnit.ConsoleRunner&version=3.9.0

//for using JSON in script
#addin nuget:?package=Cake.Json
#addin nuget:?package=Newtonsoft.Json&version=9.0.1

//for running NUnit tests in script
#addin nuget:?package=Cake.SoftNUnit3

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

NUnit3Settings nunitSettings = new NUnit3Settings { NoResults = false };

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./pathToDirectory") + Directory(configuration);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("./pathToSln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{ 
      MSBuild("./pathToSln", settings =>
        settings.SetConfiguration(configuration));
});

    
Task("Run-Automation-Tests")
    .IsDependentOn("Build")
    .Does(() =>
    {   
        SoftNUnit3($"{buildDir}/Tests.DevDest.dll", nunitSettings);
    });

Task("Rerun-Automation-Tests")
    .IsDependentOn("Run-Automation-Tests")
    .Does(async () =>
    {
        var resultPaths = nunitSettings.Results?.Count != 0
            ? nunitSettings.Results.Select(r => r.FileName)
            : new[] { FilePath.FromString("TestResult.xml") };

        for (int i = 0; i < rerunCount; i++)
        {
            //getting non passed tests from testresult xml file.
            var failed = GetNUnit3NonPassedTests(resultPaths);

            if(failed.Count().Equals(0))
            {
                break;
            }

            var testList = CreateFile($"rerun{i}.txt");
            System.IO.File.WriteAllLines(testList.FullPath, failed);

            nunitSettings.TestList = testList;
            nunitSettings.Where = null;

            SoftNUnit3($"{buildDir}/Tests.DevDest.dll", nunitSettings);
        }
    });

Task("Default")
    .IsDependentOn("Rerun-Automation-Tests");

RunTarget(target);
```
