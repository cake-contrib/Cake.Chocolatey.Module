using Cake.Core.IO;
using Cake.Core.Packaging;
using System.Collections.Generic;

namespace Cake.Chocolatey.Module
{
    /// <summary>
    /// Represents a file locator for Chocolatey packages that returns relevant
    /// files given the resource type.
    /// </summary>
    public interface IChocolateyContentResolver
    {
        /// <summary>
        /// Gets the relevant files for a Chocolatey package
        /// given a resource type.
        /// </summary>
        /// <param name="package">The package.</param>
        /// <param name="type">The resource type.</param>
        /// <returns>A collection of files.</returns>
        IReadOnlyCollection<IFile> GetFiles(PackageReference package, PackageType type);
    }
}