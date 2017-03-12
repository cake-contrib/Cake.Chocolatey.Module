using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Packaging;

namespace Cake.Chocolatey.Module
{
    /// <summary>
    /// Locates and lists contents of Chocolatey Packages.
    /// </summary>
    public class ChocolateyContentResolver : IChocolateyContentResolver
    {
        private readonly IFileSystem _fileSystem;
        private readonly ICakeEnvironment _environment;
        private readonly IGlobber _globber;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChocolateyContentResolver"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="globber">The Globber.</param>
        public ChocolateyContentResolver(
            IFileSystem fileSystem,
            ICakeEnvironment environment,
            IGlobber globber)
        {
            _fileSystem = fileSystem;
            _environment = environment;
            _globber = globber;
        }

        /// <summary>
        /// Collects all the files for the given Chocolatey Package.
        /// </summary>
        /// <param name="package">The Chocolatey Package.</param>
        /// <param name="type">The type of Chocolatey Package.</param>
        /// <returns>All the files for the Package.</returns>
        public IReadOnlyCollection<IFile> GetFiles(PackageReference package, PackageType type)
        {
            if (type == PackageType.Addin)
            {
                throw new InvalidOperationException("Chocolatey Module does not support Addins'");
            }

            if (type == PackageType.Tool)
            {
                return GetToolFiles(package);
            }

            throw new InvalidOperationException("Unknown resource type.");
        }

        private IReadOnlyCollection<IFile> GetToolFiles(PackageReference package)
        {
            var result = new List<IFile>();
            var chocoInstallLocation = _environment.GetEnvironmentVariable("ChocolateyInstall");

            if (string.IsNullOrWhiteSpace(chocoInstallLocation))
            {
                throw new InvalidOperationException("Chocolatey does not appear to be installed.");
            }

            var toolDirectory = _fileSystem.GetDirectory(chocoInstallLocation + "/lib/" + package.Package);

            if (toolDirectory.Exists)
            {
                result.AddRange(GetFiles(toolDirectory.Path.FullPath, package));
            }

            return result;
        }

        private IEnumerable<IFile> GetFiles(DirectoryPath path, PackageReference package)
        {
            var collection = new FilePathCollection(new PathComparer(_environment));

            // Get default files (exe and dll).
            var patterns = new[] { path.FullPath + "/**/*.ps1", path.FullPath + "/**/*.nuspec" };
            foreach (var pattern in patterns)
            {
                collection.Add(_globber.GetFiles(pattern));
            }

            // Include files.
            if (package.Parameters.ContainsKey("include"))
            {
                foreach (var include in package.Parameters["include"])
                {
                    var includePath = string.Concat(path.FullPath, "/", include.TrimStart('/'));
                    collection.Add(_globber.GetFiles(includePath));
                }
            }

            // Exclude files.
            if (package.Parameters.ContainsKey("exclude"))
            {
                foreach (var exclude in package.Parameters["exclude"])
                {
                    var excludePath = string.Concat(path.FullPath, "/", exclude.TrimStart('/'));
                    collection.Remove(_globber.GetFiles(excludePath));
                }
            }

            // Return the files.
            return collection.Select(p => _fileSystem.GetFile(p)).ToArray();
        }
    }
}