using System;
using System.Collections.Generic;
using Cake.Core.IO;
using Cake.Core.Packaging;
using Cake.Core;
using System.Linq;

namespace Cake.Chocolatey.Module
{
    public class ChocolateyContentResolver : IChocolateyContentResolver
    {
        private readonly IFileSystem _fileSystem;
        private readonly ICakeEnvironment _environment;
        private readonly IGlobber _globber;

        public ChocolateyContentResolver(
            IFileSystem fileSystem,
            ICakeEnvironment environment,
            IGlobber globber)
        {
            _fileSystem = fileSystem;
            _environment = environment;
            _globber = globber;
        }

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