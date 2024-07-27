
const string tfm = "net6.0";

private DirectoryPath GetModulesPath()
{
    var env = Context.Environment;
    var root = Directory(".").Path.MakeAbsolute(env);
    return Context.Configuration.GetModulePath(root, env);
}

private DirectoryPath GetToolPath()
{
    var env = Context.Environment;
    var root = Directory(".").Path.MakeAbsolute(env);
    return Context.Configuration.GetToolPath(root, env);
}

Task("Build-Module")
    .Does(() =>
{
    // dotnet clean will only clean things that will be creted by dotnet build. "old" files will not be removed.
    CleanDirectory("../../Source/Cake.Chocolatey.Module/bin");
    DotNetBuild("../../Source/Cake.Chocolatey.Module/Cake.Chocolatey.Module.csproj");
    var moduleFolder = GetModulesPath();
    if(!DirectoryExists(moduleFolder))
    {
        CreateDirectory(moduleFolder);
    }

    var target = moduleFolder + $"/Cake.Chocolatey.Module.0.0.0/lib/{tfm}";
    System.IO.File.WriteAllText(".toolpath.env", GetToolPath().FullPath);
    CleanDirectory(target); // this will NOT WORK if the dll existed before cake was started.
    CopyFiles($"../../Source/Cake.Chocolatey.Module/bin/Debug/{tfm}/Cake.Chocolatey.Module.dll", target);
    Information("Module copied to " + target);
});

RunTarget("Build-Module");