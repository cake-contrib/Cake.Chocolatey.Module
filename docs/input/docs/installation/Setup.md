Due to the fact that Cake Modules are extending and altering the internals of the Cake, the Module Assembly needs to be loaded prior to the main Cake execution.  As a result, the only place that this can really happen is during the bootstrapping phase.  If you use the [latest version of the default bootstrapper](https://github.com/cake-build/resources/commit/3d112812353d714dad142c41f170b949f4a2eb2f) this process is made very easy.  All you need to do is the following.

1. Add a modules folder into your tools folder
2. Add a packages.config file into the newly created modules folder
3. Add the name and version of the Module that you want to use.  **NOTE:** At this point, the assumption is that you are hosting the Module on NuGet.org.  If this is not the case, then additional steps would be required.
4. An example packages.config file is shown below:

```
<?xml version="1.0" encoding="utf-8"?>
<packages>
	<package id="Cake.Chocolatey.Module" version="0.1.0" />
</packages>
```

5. Run the build as normal.  During the Cake Execution, it will recognise the Module Assembly which as been restored into the `tools/modules` folder, and load it.

**NOTE:** Similar to the recommendation [here](http://cakebuild.net/docs/tutorials/getting-started) regarding only checking in your packages.config and not the entire contents of the Cake tools folder, the same recommendation is applied here.  Only check-in the packages.config file in the modules folder, and not the entire contents.