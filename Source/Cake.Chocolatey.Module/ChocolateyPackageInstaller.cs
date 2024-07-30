using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cake.Core;
using Cake.Core.Configuration;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Core.Packaging;

namespace Cake.Chocolatey.Module
{
    /// <summary>
    /// Installer for Chocolatey Packages.
    /// </summary>
    public sealed class ChocolateyPackageInstaller : IPackageInstaller
    {
        private readonly ICakeEnvironment _environment;
        private readonly IProcessRunner _processRunner;
        private readonly ICakeLog _log;
        private readonly IChocolateyContentResolver _contentResolver;
        private readonly ICakeConfiguration _config;
        private readonly IFileSystem _fileSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChocolateyPackageInstaller"/> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="processRunner">The process runner.</param>
        /// <param name="log">The log.</param>
        /// <param name="contentResolver">The Chocolatey Package Content Resolver.</param>
        /// <param name="config">the configuration.</param>
        /// <param name="fileSystem">the fileSystem.</param>
        public ChocolateyPackageInstaller(ICakeEnvironment environment, IProcessRunner processRunner, ICakeLog log, IChocolateyContentResolver contentResolver, ICakeConfiguration config, IFileSystem fileSystem)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _processRunner = processRunner ?? throw new ArgumentNullException(nameof(processRunner));
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _contentResolver = contentResolver ?? throw new ArgumentNullException(nameof(contentResolver));
            _config = config;
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        /// <summary>
        /// Determines whether this instance can install the specified resource.
        /// </summary>
        /// <param name="package">The package reference.</param>
        /// <param name="type">The package type.</param>
        /// <returns><c>true</c> if this installer can install the specified resource; otherwise <c>false</c>.</returns>
        public bool CanInstall(PackageReference package, PackageType type)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            return package.Scheme.Equals("choco", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Installs the specified resource at the given location.
        /// </summary>
        /// <param name="package">The package reference.</param>
        /// <param name="type">The package type.</param>
        /// <param name="path">The location where to install the package.</param>
        /// <returns>The installed files.</returns>
        public IReadOnlyCollection<IFile> Install(PackageReference package, PackageType type, DirectoryPath path)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            path = path.MakeAbsolute(_environment);

            if (!_environment.Platform.IsWindows())
            {
                // choco runs only on windows!
                _log.Warning("Unable to install {0}, since the choco-scheme can only install on Windows. Ensure {0} exists on your system!", package.Package);

                // we need to return something, or else the installation counts as "failed".
                var noInstallFilePath = path.CombineWithFilePath("no-choco-install.txt");
                var noInstallFile = _fileSystem.GetFile(noInstallFilePath);
                using (var stream = noInstallFile.Open(FileMode.Append))
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine("{0} not installed, since we're not running on windows.", package.Package);
                }

                return new[] { noInstallFile };
            }

            // Install the package.
            _log.Debug("Installing Chocolatey package {0}...", package.Package);
            var process = _processRunner.Start(
                "choco",
                new ProcessSettings { Arguments = GetArguments(package, path, _config), RedirectStandardOutput = true, Silent = _log.Verbosity < Verbosity.Diagnostic });

            process.WaitForExit();

            var exitCode = process.GetExitCode();
            if (exitCode != 0)
            {
                _log.Warning("Chocolatey exited with {0}", exitCode);
                var output = string.Join(Environment.NewLine, process.GetStandardOutput());
                _log.Verbose(Verbosity.Diagnostic, "Output:\r\n{0}", output);
            }

            var result = _contentResolver.GetFiles(package, type);
            if (result.Count != 0)
            {
                return result;
            }

            // TODO: maybe some warnings here
            return result;
        }

        private static ProcessArgumentBuilder GetArguments(
            PackageReference definition,
            DirectoryPath installationRoot,
            ICakeConfiguration config)
        {
            var arguments = new ProcessArgumentBuilder();

            arguments.Append("install");
            arguments.AppendQuoted(definition.Package);
            arguments.Append("-y");

            // if an absolute uri is specified for source, use this
            // otherwise check config for customise package source/s
            if (definition.Address != null)
            {
                arguments.Append("--source");
                arguments.AppendQuoted(definition.Address.AbsoluteUri);
            }
            else
            {
                var chocolateySource = config.GetValue(Constants.Chocolatey.Source) ?? "https://chocolatey.org/api/v2/";
                if (!string.IsNullOrWhiteSpace(chocolateySource))
                {
                    arguments.Append("--source");
                    arguments.AppendQuoted(chocolateySource);
                }
            }

            // Version
            if (definition.Parameters.ContainsKey("version"))
            {
                arguments.Append("--version");
                arguments.AppendQuoted(definition.Parameters["version"].First());
            }

            // Prerelease
            if (definition.Parameters.ContainsKey("prerelease"))
            {
                arguments.Append("--prerelease");
            }

            return arguments;
        }
    }
}
