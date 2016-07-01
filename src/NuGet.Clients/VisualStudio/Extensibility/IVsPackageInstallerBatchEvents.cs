using System.Runtime.InteropServices;

namespace NuGet.VisualStudio
{
    /// <summary>
    /// Contains batch events which are raised when packages are installed or uninstalled from projects and the current
    /// solution.
    /// </summary>
    [ComImport]
    [Guid("3b76690b-eb3e-4cfa-b5af-6574c567c842")]
    public interface IVsPackageInstallerBatchEvents
    {
        /// <summary>
        /// Raised when installing/ uninstalling packages in a batch starts for current project
        /// </summary>
        event VsPackageBatchEventHandler StartBatchProcessing;

        /// <summary>
        /// Raised when installing/ uninstalling packages in a batch ends for current project
        /// </summary>
        event VsPackageBatchEventHandler EndBatchProcessing;

    }
}