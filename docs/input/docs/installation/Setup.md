---
Title: Setup
---

# Installation

Due to the fact that Cake Modules are extending and altering the internals of Cake, the module assembly needs to be loaded prior to the main Cake execution. As a result, the only place that this can really happen is during the bootstrapping phase. If you use the latest version of the default bootstrapper this process is made very easy. All you need to do is the following.

1. Add the following line in you're cake.recipe:

   ```cs
   #module nuget:?package=Cake.Chocolatey.Module&version=<version>
   ```
1. For Cake versions before `1.0.0`, run the build with argument `--bootstrap` (i.e. `./build.ps1 --bootstrap`).

   This will restore the module assembly into the `tools/Modules` folder
1. Run the build as normal. During Cake's execution, it will recognize the module assembly which has been restored into the `tools/Modules` folder, and load it.

:::{.alert .alert-info}
**NOTE**

These steps assume you are using the default package source repository (community feed). Custom feeds require additional steps.
:::
