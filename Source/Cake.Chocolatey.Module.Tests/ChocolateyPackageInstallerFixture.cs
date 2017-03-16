using Cake.Core;
using Cake.Core.Configuration;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Core.Packaging;
using Cake.Testing;
using NSubstitute;
using System.Collections.Generic;

namespace Cake.Chocolatey.Module.Tests
{
    /// <summary>
    /// Fixture used for testing ChocolatyePackageInstaller
    /// </summary>
    internal sealed class ChocolateyPackageInstallerFixture
    {
        public ICakeEnvironment Environment { get; set; }
        public IFileSystem FileSystem { get; set; }
        public IProcessRunner ProcessRunner { get; set; }
        public IChocolateyContentResolver ContentResolver { get; set; }
        public ICakeLog Log { get; set; }

        public PackageReference Package { get; set; }
        public PackageType PackageType { get; set; }
        public DirectoryPath InstallPath { get; set; }

        public ICakeConfiguration Config { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChocolateyPackageInstallerFixture"/> class.
        /// </summary>
        internal ChocolateyPackageInstallerFixture()
        {
            Environment = FakeEnvironment.CreateUnixEnvironment();
            FileSystem = new FakeFileSystem(Environment);
            ProcessRunner = Substitute.For<IProcessRunner>();
            ContentResolver = Substitute.For<IChocolateyContentResolver>();
            Log = new FakeLog();
            Config = Substitute.For<ICakeConfiguration>();
            Package = new PackageReference("choco:?package=windirstat");
            PackageType = PackageType.Addin;
            InstallPath = new DirectoryPath("./chocolatey");
        }

        /// <summary>
        /// Create the installer.
        /// </summary>
        /// <returns>The chocolatey package installer.</returns>
        internal ChocolateyPackageInstaller CreateInstaller()
        {
            return new ChocolateyPackageInstaller(Environment, ProcessRunner, Log, ContentResolver, Config);
        }

        /// <summary>ChocolateyPackageInstallerFixture
        /// Installs the specified resource at the given location.
        /// </summary>
        /// <returns>The installed files.</returns>
        internal IReadOnlyCollection<IFile> Install()
        {
            var installer = CreateInstaller();
            return installer.Install(Package, PackageType, InstallPath);
        }

        /// <summary>
        /// Determines whether this instance can install the specified resource.
        /// </summary>
        /// <returns><c>true</c> if this installer can install the specified resource; otherwise <c>false</c>.</returns>
        internal bool CanInstall()
        {
            var installer = CreateInstaller();
            return installer.CanInstall(Package, PackageType);
        }
    }
}