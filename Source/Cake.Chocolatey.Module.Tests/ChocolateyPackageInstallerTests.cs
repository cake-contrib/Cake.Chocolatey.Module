using Cake.Core.Packaging;
using NSubstitute;
using System;

using Cake.Testing.Xunit;

using Xunit;

namespace Cake.Chocolatey.Module.Tests
{
    /*
     * Most of the WindowsFactAttributes should be "normal" FactAttributes
     * once https://github.com/cake-build/cake/issues/4322 is resolved.
     */

    /// <summary>
    /// ChocolateyPackageInstaller unit tests
    /// </summary>
    public sealed class ChocolateyPackageInstallerTests
    {
        public sealed class TheConstructor
        {
            [WindowsFact]
            public void Should_Throw_If_Environment_Is_Null()
            {
                // Given
                var fixture = ChocolateyPackageInstallerFixture.Windows;
                fixture.Environment = null;

                // When
                var result = Record.Exception(() => fixture.CreateInstaller());

                // Then
                Assert.IsType<ArgumentNullException>(result);
                Assert.Equal("environment", ((ArgumentNullException)result).ParamName);
            }

            [WindowsFact]
            public void Should_Throw_If_Process_Runner_Is_Null()
            {
                // Given
                var fixture = ChocolateyPackageInstallerFixture.Windows;
                fixture.ProcessRunner = null;

                // When
                var result = Record.Exception(() => fixture.CreateInstaller());

                // Then
                Assert.IsType<ArgumentNullException>(result);
                Assert.Equal("processRunner", ((ArgumentNullException)result).ParamName);
            }

            [WindowsFact]
            public void Should_Throw_If_Content_Resolver_Is_Null()
            {
                // Given
                var fixture = ChocolateyPackageInstallerFixture.Windows;
                fixture.ContentResolver = null;

                // When
                var result = Record.Exception(() => fixture.CreateInstaller());

                // Then
                Assert.IsType<ArgumentNullException>(result);
                Assert.Equal("contentResolver", ((ArgumentNullException)result).ParamName);
            }

            [WindowsFact]
            public void Should_Throw_If_Log_Is_Null()
            {
                // Given
                var fixture = ChocolateyPackageInstallerFixture.Windows;
                fixture.Log = null;

                // When
                var result = Record.Exception(() => fixture.CreateInstaller());

                // Then
                Assert.IsType<ArgumentNullException>(result);
                Assert.Equal("log", ((ArgumentNullException)result).ParamName);
            }
        }

        public sealed class TheCanInstallMethod
        {
            private string CHOCOLATEY_CONFIGKEY = "Chocolatey_Source";

            [WindowsFact]
            public void Should_Throw_If_URI_Is_Null()
            {
                // Given
                var fixture = ChocolateyPackageInstallerFixture.Windows;
                fixture.Package = null;

                // When
                var result = Record.Exception(() => fixture.CanInstall());

                // Then
                Assert.IsType<ArgumentNullException>(result);
                Assert.Equal("package", ((ArgumentNullException)result).ParamName);
            }

            [WindowsFact]
            public void Should_Be_Able_To_Install_If_Scheme_Is_Correct()
            {
                // Given
                var fixture = ChocolateyPackageInstallerFixture.Windows;
                fixture.Package = new PackageReference("choco:?package=windristat");

                // When
                var result = fixture.CanInstall();

                // Then
                Assert.True(result);
            }

            [WindowsFact]
            public void Should_Not_Be_Able_To_Install_If_Scheme_Is_Incorrect()
            {
                // Given
                var fixture = ChocolateyPackageInstallerFixture.Windows;
                fixture.Package = new PackageReference("homebrew:?package=windirstat");

                // When
                var result = fixture.CanInstall();

                // Then
                Assert.False(result);
            }

            [WindowsFact]
            public void Should_Ignore_Custom_Source_If_AbsoluteUri_Is_Used()
            {
                var fixture = ChocolateyPackageInstallerFixture.Windows;
                fixture.Package = new PackageReference("choco:http://absolute/?package=windirstat");

                // When
                var result = Record.Exception(() => fixture.Install());

                // Then
                Assert.Null(result);
                fixture.Config.DidNotReceive().GetValue(CHOCOLATEY_CONFIGKEY);
            }

            [WindowsFact]
            public void Should_Use_Custom_Source_If_RelativeUri_Is_Used()
            {
                var fixture = ChocolateyPackageInstallerFixture.Windows;
                fixture.Package = new PackageReference("choco:?package=windirstat");

                // When
                var result = Record.Exception(() => fixture.Install());

                // Then
                Assert.Null(result);
                fixture.Config.Received().GetValue(CHOCOLATEY_CONFIGKEY);
            }
        }

        public sealed class TheInstallMethod
        {
            [WindowsFact]
            public void Should_Throw_If_Uri_Is_Null()
            {
                // Given
                var fixture = ChocolateyPackageInstallerFixture.Windows;
                fixture.Package = null;

                // When
                var result = Record.Exception(() => fixture.Install());

                // Then
                Assert.IsType<ArgumentNullException>(result);
                Assert.Equal("package", ((ArgumentNullException)result).ParamName);
            }

            [WindowsFact]
            public void Should_Throw_If_Install_Path_Is_Null()
            {
                // Given
                var fixture = ChocolateyPackageInstallerFixture.Windows;
                fixture.InstallPath = null;

                // When
                var result = Record.Exception(() => fixture.Install());

                // Then
                Assert.IsType<ArgumentNullException>(result);
                Assert.Equal("path", ((ArgumentNullException)result).ParamName);
            }

            [Fact]
            public void On_Non_Windows_Install_Does_Not_Fail()
            {
                var fixture = ChocolateyPackageInstallerFixture.Unix;
                fixture.Package = new PackageReference("choco:?package=windirstat");

                // When
                var result = fixture.Install();

                // Then
                Assert.Single(result);
            }
        }
    }
}
