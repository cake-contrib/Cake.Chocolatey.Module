// do not reference Cake.Chocolatey.Module.dll directly, prepare.cake will place it in the modules folder
#tool choco:?package=cake-demo&version=0.1.0

var target = Argument("target", "Build");

Task("Build")
    .Does(() =>
{
    StartProcess("hello.exe");
});

RunTarget(target);