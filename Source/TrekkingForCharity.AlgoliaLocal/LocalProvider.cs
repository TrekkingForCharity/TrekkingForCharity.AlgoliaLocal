// Copyright (c) Trekking for Charity. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Net.Http;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;
using TrekkingForCharity.AlgoliaLocal.Infrastructure;

namespace TrekkingForCharity.AlgoliaLocal
{
    public class LocalProvider : ILocalProvider
    {
        private readonly MockHttpMessageHandler _httpMessageHandler;

        public LocalProvider()
        {
            this._httpMessageHandler = new MockHttpMessageHandler();
            this._httpMessageHandler.When("*").Respond(handler: this.Handler);
        }

        public MockHttpMessageHandler HttpMessageHandler => this._httpMessageHandler;

        private Task<HttpResponseMessage> Handler(HttpRequestMessage message)
        {
            throw new NotImplementedException($"No route setup for {message.RequestUri.AbsolutePath} with the method {message.Method}");
        }
    }
}