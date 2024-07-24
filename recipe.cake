#load nuget:?package=Cake.Recipe&version=3.1.1

Environment.SetVariableNames();

BuildParameters.SetParameters(context: Context,
                            buildSystem: BuildSystem,
                            sourceDirectoryPath: "./Source",
                            title: "Cake.Chocolatey.Module",
                            repositoryOwner: "cake-contrib",
                            repositoryName: "Cake.Chocolatey.Module",
                            shouldRunCodecov: false,
                            shouldRunCoveralls: false,
                            shouldPostToGitter: false,
                            appVeyorAccountName: "cakecontrib",
                            shouldRunDotNetCorePack: true,
                            preferredBuildProviderType: BuildProviderType.GitHubActions,
                            preferredBuildAgentOperatingSystem: PlatformFamily.Windows);

BuildParameters.PrintParameters(Context);

ToolSettings.SetToolPreprocessorDirectives(
                            gitReleaseManagerGlobalTool: "#tool dotnet:?package=GitReleaseManager.Tool&version=0.18.0");

ToolSettings.SetToolSettings(context: Context,
                            testCoverageFilter: "+[*]* -[xunit.*]* -[Cake.Core]* -[Cake.Testing]* -[*.Tests]*",
                            testCoverageExcludeByAttribute: "*.ExcludeFromCodeCoverage*",
                            testCoverageExcludeByFile: "*/*Designer.cs;*/*.g.cs;*/*.g.i.cs");

Build.RunDotNetCore();

