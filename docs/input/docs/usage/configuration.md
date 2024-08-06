---
Title: Configuration
---

As mentioned in the parameters section, it is possible to specify the specific source that is used when installing a tool.  If you always use the same alternative source, doing this for each source can become unwieldy.  Instead, it is possible to configure a default source which is used for all tool installations made by the Cake.Chocolatey.Module.

This makes use of the configuration options within Cake which are documented [here](https://cakebuild.net/docs/running-builds/configuration/set-configuration-values).

For Cake.Chocolatey.Module, the additional configuration option can be used:

# Chocolatey Download Url

This allows the control of where Cake downloads Chocolatey packages from when using the tool preprocessor directive.  This can be useful when it is necessary to work in an offline mode, where direct access to chocolatey.org is not available.

## Default Value

```text
https://chocolatey.org/api/v2/
```

## Environment Variable Name

```text
CHOCOLATEY_SOURCE
```

## ini File Contents

```ini
[Chocolatey]
Source=http://myfeed/chocolatey/
```

## Direct Argument

```sh
cake.exe --chocolatey_source=http://myfeed/chocolatey/
```
