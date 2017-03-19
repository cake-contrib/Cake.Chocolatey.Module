The folllowing URI parameters are supported by the Cake.Chocolatey.Module.

# Source

This is not a named parameter, but it is permitted as per the URI definition.  By default, Chocolatey will attempt to install applications from the Chocolatey Community Feed located at https://chocolatey.org/api/v2/.  If your package is actually hosted on another feed, for example, on a MyGet feed, the installation source can be overridden.

## Example

```
#tool choco:https://www.myget.org/F/gep13/api/v2?package=gep13.gitConfig
```

# Package

This is the name of the Chocolatey package that you would like to install.  This should match the ID property as defined in the nuspec file for the Chocolatey package.

## Example

```
#tool choco:?package=nodejs.install
```

# Version

The specific version of the application that is being installed.  If not provided, Chocolatey will install the latest package version that is available.

## Example

```
#tool choco:?package=nodejs.install&version=7.7.3
```

# Pre-Release

This controls whether or not Chocolatey is allowed to install pre-release versions of the application that may exist in the package source that is being used.  By default, this is not allowed.

## Example

```
#tool choco:?package=nodejs.install&version=7.7.3&prerelease
```