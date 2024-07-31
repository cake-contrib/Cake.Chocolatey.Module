var target = Argument("target", "Default");

Task("Clean")
  .Does(() =>
{
    DeleteFiles("**/*.exe");
    DeleteFiles("**/*.nupkg");
});

Task("Build-EXEs")
  .DoesForEach(GetFiles("**/*.rs"), (f) =>
{
    using(var p = StartAndReturnProcess("rustc", new ProcessSettings
    {
        Arguments = f.FullPath
    }))
    {
        p.WaitForExit();
        if(p.GetExitCode() != 0)
        {
            throw new Exception($"Failed to compile {f.FullPath}");
        }

        var exe = f.GetFilenameWithoutExtension() + ".exe";
        MoveFile(exe, $"cake-demo/tools/{exe}");
    }
});

Task("Build-Choco")
  .IsDependentOn("Build-EXEs")
  .Does(() =>
{
    using(var p = StartAndReturnProcess("choco", new ProcessSettings
    {
        Arguments = "pack",
        WorkingDirectory = "cake-demo",
    }))
    {
        p.WaitForExit();
        if(p.GetExitCode() != 0)
        {
            throw new Exception("Failed to pack choco package");
        }

        MoveFiles(GetFiles("**/*.nupkg"), "./");
    }
});

Task("Default")
 .IsDependentOn("Clean")
 .IsDependentOn("Build-Choco");

RunTarget(target);