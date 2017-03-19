---
Title: New Release - 0.1.0
Published: 18/3/2017
Category: Release
Author: gep13
---

# Announcing the 0.1.0 release of Cake.Chocolatey.Module

We are very pleased to announce this initial release of the Cake.Chocolatey.Module.

This included basic support for installing applications as part of your build process using the familiar pre-processor directive syntax used when installing tools and addins from NuGet.org.

An example of how this is achieved is shown below:

```
#tool choco:?package=nodejs.install&version=7.7.3
```

Check out the documentation for more information.

Please do not hesitate to reach out in the [Gitter Channel](https://gitter.im/cake-contrib/Lobby) if you have any issues using this addin.