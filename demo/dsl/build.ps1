Set-Location -LiteralPath $PSScriptRoot

$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
$env:DOTNET_NOLOGO = '1'

dotnet tool restore
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

@("tools", ".cake") | % {
    if (Test-Path $_) {
        Remove-Item -Recurse -Force $_
    }
}

Write-Host "`npreparing the module`n"
dotnet cake prepare.cake
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

# Set the choco dir to somethin local, so installing the test package
# does not screw up your system. We'll need this only on windows
# so, adding it to the .ps1 is probably enough.
$toolPath = "$(pwd)/tools"
if (Test-Path .toolpath.env) {
    # that file is written by the prepare.cake script
    $toolPath = Get-Content .toolpath.env
}
$chocoPath = "$toolPath/choco"
Write-Host "`nSetting ChocolateyInstallDir to $chocoPath`n"
Remove-Item -Path $chocoPath -Force -ErrorAction Ignore
$env:ChocolateyInstallDir = "$chocoPath"
Write-Host "`nNow running the build script`n"
dotnet cake build.cake @args
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
