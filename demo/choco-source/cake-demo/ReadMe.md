## Summary

This is the test-package, to be used in tests with Cake.Chocolatey.Module.
It contains the following parts:

- The choco package
- hello-world.exe
  This exe does nothing really, but it can be used as a simple tool in tests.
- build.cake
  This script builds the hello-world.exe (using rust) and then creates the choco package
  The choco package is "checked in" - so it does not need to be re-created every time.
