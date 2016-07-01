using System;
using System.ComponentModel.Composition;
using NuGet.ProjectManagement;

namespace NuGet.VisualStudio.Implementation.Extensibility
{
    /// <summary>
    /// Contains batch events to be raised when performing multiple packages install/ uninstall
    /// </summary>
    [Export(typeof(IVsPackageInstallerBatchEvents))]
    [Export(typeof(VsPackageInstallerBatchEvents))]
    public class VsPackageInstallerBatchEvents : IVsPackageInstallerBatchEvents
    {
        public event VsPackageBatchEventHandler StartBatchProcessing;

        public event VsPackageBatchEventHandler EndBatchProcessing;

        private readonly PackageEvents _eventSource;

        [ImportingConstructor]
        public VsPackageInstallerBatchEvents(IPackageEventsProvider eventProvider)
        {
            _eventSource = eventProvider.GetPackageEvents();

            _eventSource.StartBatchProcessing += NotifyBatchStart;
            _eventSource.EndBatchProcessing += NotifyBatchEnd;
        }

        private void NotifyBatchStart(object sender, EventArgs e)
        {
            StartBatchProcessing?.Invoke();
        }

        private void NotifyBatchEnd(object sender, EventArgs e)
        {
            EndBatchProcessing?.Invoke();
        }

    }
}
