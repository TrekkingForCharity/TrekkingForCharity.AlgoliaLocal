// Copyright (c) Trekking for Charity. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;
using TrekkingForCharity.AlgoliaLocal.Infrastructure;

namespace TrekkingForCharity.AlgoliaLocal
{
    public class LocalProvider : ILocalProvider
    {
        private readonly MockHttpMessageHandler _httpMessageHandler;

        private readonly RouteProcessorContainer _routeProcessorContainer;

        public LocalProvider(IDataRepository dataRepository)
        {
            this._routeProcessorContainer = new RouteProcessorContainer(dataRepository);
            this._httpMessageHandler = new MockHttpMessageHandler();
            this._httpMessageHandler.When("*").Respond(handler: this.Handler);
        }

        public MockHttpMessageHandler HttpMessageHandler => this._httpMessageHandler;

        public RouteProcessorContainer RouteProcessorContainer => this._routeProcessorContainer;

        private async Task<HttpResponseMessage> Handler(HttpRequestMessage message)
        {
            var routeResult = this._routeProcessorContainer.GetRouteForPath(message.RequestUri.AbsolutePath);
            if (!routeResult.IsSuccess)
            {
                return new HttpResponseMessage(HttpStatusCode.NotImplemented);
            }

            var processResult = await routeResult.Value.Process(message);
            if (processResult.IsSuccess)
            {
                return processResult.Value;
            }

            return new HttpResponseMessage(HttpStatusCode.NotImplemented);
        }
    }
}