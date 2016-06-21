// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Threading;
using NuGet.ProjectManagement;
using NuGet.Protocol;

namespace NuGet.PackageManagement.UI
{
    public class PackageSourceDiagnosticsWorker
    {
        private readonly INuGetUI _uiService;
        private JoinableTask _task;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public PackageSourceDiagnosticsWorker(INuGetUI uiService)
        {
            if (uiService == null)
            {
                throw new ArgumentNullException(nameof(uiService));
            }

            _uiService = uiService;
        }

        public void Start(CancellationToken cancellationToken)
        {
            _task = NuGetUIThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await NuGetUIThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                _uiService.ProgressWindow.Log(MessageLevel.Info, "Bazinga!");

                while (!_cts.Token.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5.5), _cts.Token);

                    foreach (var sourceRepository in _uiService.ActiveSources)
                    {
                        var dr = await sourceRepository.GetResourceAsync<PackageSourceDiagnosticsResource>(_cts.Token);
                        var d = dr.PackageSourceDiagnostics;

                        foreach (var msg in d.DiagnosticMessages)
                        {
                            _uiService.ProgressWindow.Log(MessageLevel.Warning, $"[PSD] {msg.Details}");
                        }
                    }
                }
            });
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cts.Cancel();
            await _task?.JoinAsync(cancellationToken);
        }
    }
}
