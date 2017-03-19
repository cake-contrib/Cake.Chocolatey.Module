Installing a tool using the Chocolatey Cake Module is as simple as:

```
#tool choco:?package=nodejs.install&version=7.7.3
```

If the tool in question comes from a different source, you can change that as follows:

```
#tool choco:https://www.myget.org/F/gep13/api/v2?package=gep13.gitConfig&version=1.0.6
```

In this version 0.1.0 release of the Chocolatey Cake Module you can also specify whether the package in question is a pre-release or not:

```
#tool choco:?package=packageA&prerelease
```

Future versions of the Chocolatey Cake Module will include additional options for installing the package, including things like `--forcex86`, `--install-arguments`, `--package-parameters`, etc.  This initial release includes just the very basic of installation functionality.