using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ChocolateyPackageInstaller"/> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="processRunner">The process runner.</param>
        /// <param name="log">The log.</param>
        /// <param name="contentResolver">The Chocolatey Package Content Resolver.</param>
        /// <param name="config">the configuration.</param>
        public ChocolateyPackageInstaller(ICakeEnvironment environment, IProcessRunner processRunner, ICakeLog log, IChocolateyContentResolver contentResolver, ICakeConfiguration config)
        {
            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            if (processRunner == null)
            {
                throw new ArgumentNullException(nameof(processRunner));
            }

            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }

            if (contentResolver == null)
            {
                throw new ArgumentNullException(nameof(contentResolver));
            }

            _environment = environment;
            _processRunner = processRunner;
            _log = log;
            _contentResolver = contentResolver;
            _config = config;
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
