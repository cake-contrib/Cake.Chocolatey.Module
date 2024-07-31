#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "${BASH_SOURCE[0]}")"

export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1

dotnet tool restore

rm -rf tools 2>/dev/null || true
rm -rf .cake 2>/dev/null || true

echo ""
echo "preparing the module"
echo ""
dotnet cake prepare.cake

echo ""
echo "Now running the build script"
echo ""
# no need to set the CakeInstallDir, as
dotnet cake build.cake "$@"
