// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NuGet.Protocol.Tests
{
    internal class LambdaMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _delegate;

        public LambdaMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            if (responseFactory == null)
            {
                throw new ArgumentNullException(nameof(responseFactory));
            }

            _delegate = (request, _) => Task.FromResult(responseFactory(request));
        }

        public LambdaMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> @delegate)
        {
            if (@delegate == null)
            {
                throw new ArgumentNullException(nameof(@delegate));
            }

            _delegate = @delegate;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _delegate(request, cancellationToken);
        }
    }
}
