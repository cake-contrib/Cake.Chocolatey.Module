using System;
using Cake.Core.Composition;
using Cake.Core.Packaging;

namespace Cake.Chocolatey.Module
{
    /// <summary>
    /// The module responsible for registering
    /// default types in the Cake.Chocolatey.Module assembly.
    /// </summary>
    public sealed class ChocolateyModule : ICakeModule
    {
        /// <summary>
        /// Performs custom registrations in the provided registrar.
        /// </summary>
        /// <param name="registrar">The container registrar.</param>
        public void Register(ICakeContainerRegistrar registrar)
        {
            if (registrar == null)
            {
                throw new ArgumentNullException(nameof(registrar));
            }

            registrar.RegisterType<ChocolateyPackageInstaller>().As<IPackageInstaller>().Singleton();
            registrar.RegisterType<ChocolateyContentResolver>().As<IChocolateyContentResolver>().Singleton();
        }
    }
}