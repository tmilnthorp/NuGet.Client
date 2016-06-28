// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Net.Http;

namespace NuGet.Protocol
{
    /// <summary>
    /// A request to be handled by <see cref="HttpRetryHandler"/>. This type should contain all
    /// of the knowledge necessary to make a request, while handling transient transport errors.
    /// </summary>
    public class HttpRetryHandlerRequest
    {
        public static readonly TimeSpan DefaultDownloadTimeout = TimeSpan.FromSeconds(60);
        public static readonly TimeSpan DefaultRequestTimeout = TimeSpan.FromSeconds(100);
        public static readonly TimeSpan DefaultRetryDelay = TimeSpan.FromMilliseconds(200);

        public HttpRetryHandlerRequest(Func<HttpRequestMessage> requestFactory)
        {
            if (requestFactory == null)
            {
                throw new ArgumentNullException(nameof(requestFactory));
            }

            RequestFactory = requestFactory;
        }

        /// <summary>
        /// The factory that generates each request message. This factory is invoked for each attempt.
        /// </summary>
        public Func<HttpRequestMessage> RequestFactory { get; }

        /// <summary>The HTTP completion option to use for the next attempt.</summary>
        public HttpCompletionOption CompletionOption { get; set; } = HttpCompletionOption.ResponseHeadersRead;

        /// <summary>The maximum number of times to try the request. This value includes the initial attempt.</summary>
        /// <remarks>This API is intended only for testing purposes and should not be used in product code.</remarks>
        public int MaxTries { get; set; } = 3;

        /// <summary>How long to wait on the request to come back with a response.</summary>
        public TimeSpan RequestTimeout { get; set; } = DefaultRequestTimeout;

        /// <summary>How long to wait before trying again after a failed request.</summary>
        /// <summary>This API is intended only for testing purposes and should not be used in product code.</summary>
        public TimeSpan RetryDelay { get; set; } = DefaultRetryDelay;

        /// <summary>The timeout to apply to <see cref="DownloadTimeoutStream"/> instances.</summary>
        public TimeSpan DownloadTimeout { get; set; } = DefaultDownloadTimeout;
    }
}
