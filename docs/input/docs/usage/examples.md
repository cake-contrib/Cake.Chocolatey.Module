---
Title: Examples
---

:::{.alert .alert-warning}
**Administrative Privileges**

Keep in mind, that installing Chocolatey packages likely requires administrator privileges.
See [the chocolatey setup](https://docs.chocolatey.org/en-us/choco/setup/#non-administrative-install) for more information.
:::

Installing a tool using the Chocolatey Cake Module is as simple as:

```cs
#tool choco:?package=nodejs.install&version=7.7.3
```

If the tool in question comes from a different source, you can change that as follows:

```cs
#tool choco:https://www.myget.org/F/gep13/api/v2?package=gep13.gitConfig&version=1.0.6
```

If the tool in question is only available as a pre-release, the installation of pre-releases needs to be allowed explicitly:

```cs
#tool choco:?package=packageA&prerelease
```

:::{.alert .alert-info}
**NOTE**

Future versions of the Chocolatey Cake Module might include additional options for installing the package, including things like `--forcex86`, `--install-arguments`, `--package-parameters`, etc.

If you require an additional option for installing a package, don't hesitate to reach out via [GitHub](https://github.com/cake-contrib/Cake.Chocolatey.Module/issues).
:::
