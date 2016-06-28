// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NuGet.Configuration;
using Xunit;

namespace NuGet.Protocol.Tests
{
    public class HttpSourceDiagnosticsHandlerTests
    {
        private const string TestUrl = "https://test.local/test.json";
        private static readonly TimeSpan SmallTimeout = TimeSpan.FromMilliseconds(50);
        private static readonly TimeSpan LargeTimeout = TimeSpan.FromMilliseconds(250);

        [Fact]
        public async Task SendAsync_CancelsRequestAfterTimeout()
        {
            var packageSource = new PackageSource("http://package.source.net");
            var diagnostics = Mock.Of<IPackageSourceDiagnostics>();

            var requestToken = CancellationToken.None;
            var handler = new HttpSourceDiagnosticsHandler(packageSource, diagnostics)
            {
                InnerHandler = new LambdaMessageHandler(
                    async (_, token) =>
                    {
                        requestToken = token;
                        await Task.Delay(LargeTimeout, token);
                        return new HttpResponseMessage(HttpStatusCode.OK);
                    })
            };

            var request = new HttpRequestMessage(HttpMethod.Get, TestUrl);
            request.SetRequestTimeout(SmallTimeout);

            // Act & Assert
            await Assert.ThrowsAsync<TimeoutException>(() => SendAsync(handler, request));
            Assert.True(requestToken.CanBeCanceled);
            Assert.True(requestToken.IsCancellationRequested);
        }

        [Fact]
        public async Task SendAsync_ThrowsTimeoutExceptionForTimeout()
        {
            // Arrange
            var packageSource = new PackageSource("http://package.source.net");
            var diagnostics = Mock.Of<IPackageSourceDiagnostics>();

            var handler = new HttpSourceDiagnosticsHandler(packageSource, diagnostics)
            {
                InnerHandler = new LambdaMessageHandler(
                    async (_, token) =>
                    {
                        await Task.Delay(LargeTimeout, token);
                        return new HttpResponseMessage(HttpStatusCode.OK);
                    })
            };

            var request = new HttpRequestMessage(HttpMethod.Get, TestUrl);
            request.SetRequestTimeout(TimeSpan.Zero);

            // Act & Assert
            await Assert.ThrowsAsync<TimeoutException>(() => SendAsync(handler, request));
        }

        private static async Task<HttpResponseMessage> SendAsync(
            HttpMessageHandler handler,
            HttpRequestMessage request = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var client = new HttpClient(handler))
            {
                return await client.SendAsync(request ?? new HttpRequestMessage(HttpMethod.Get, TestUrl), cancellationToken);
            }
        }
    }
}