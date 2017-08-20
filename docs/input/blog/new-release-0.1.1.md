---
Title: New Release - 0.1.1
Published: 18/3/2017
Category: Release
Author: gep13
---

# Well, this is a little bit embarassing...

Looks like I missed an important step when committing everything for what I thought was going to be the 0.1.0 release!  I forgot to include the Cake.Chocolatey.Module.nuspec file, which is used to package up the NuGet package, and which is ultimately used as a trigger for whether a deployment to nuget.org is actually required.

This problem has now been corrected, and you can find the 0.1.1 release of the Cake.Chocolatey.Module here:

https://www.nuget.org/packages/Cake.Chocolatey.Module/

Please do not hesitate to reach out in the [Gitter Channel](https://gitter.im/cake-contrib/Lobby) if you have any issues using this addin.