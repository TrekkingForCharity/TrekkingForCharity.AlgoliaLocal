// Copyright (c) Trekking for Charity. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using ResultMonad;
using TrekkingForCharity.AlgoliaLocal.Infrastructure;
using Xunit;

namespace TrekkingForCharity.AlgoliaLocal.Tests
{
    public class LocalProviderTests
    {
        [Fact]
        public void WhenClassIsInitialized_MockHandlerIsCreated()
        {
            var dataRepository = new Mock<IDataRepository>();
            var localProvider = new LocalProvider(dataRepository.Object);

            Assert.NotNull(localProvider.HttpMessageHandler);
        }

        [Fact]
        public async Task Handler_WhenLookingForAnUnknownRoute_NotImplementedIsReturned()
        {
            var dataRepository = new Mock<IDataRepository>();
            var localProvider = new LocalProvider(dataRepository.Object);

            var client = localProvider.HttpMessageHandler.ToHttpClient();

            var response = await client.GetAsync("http://text.example.com");

            Assert.Equal(HttpStatusCode.NotImplemented, response.StatusCode);
        }

        [Fact]
        public async Task Handler_WhenProcessFails_NotImplementedIsReturned()
        {
            var dataRepository = new Mock<IDataRepository>();
            var localProvider = new LocalProvider(dataRepository.Object);
            var routeProcessor = new Mock<IRouteProcessor>();
            routeProcessor.Setup(x => x.IsMatch(It.IsAny<string>())).Returns(true);
            routeProcessor.Setup(x => x.Process(It.IsAny<HttpRequestMessage>())).ReturnsAsync(() => Result.Fail<HttpResponseMessage>());
            localProvider.RouteProcessorContainer.AddRouteProcessor(routeProcessor.Object);
            var client = localProvider.HttpMessageHandler.ToHttpClient();
            var response = await client.GetAsync("http://text.example.com");
            Assert.Equal(HttpStatusCode.NotImplemented, response.StatusCode);
        }

        [Fact]
        public async Task Handler_WhenProcessSucceeds_ValidResultIsReturned()
        {
            var dataRepository = new Mock<IDataRepository>();
            var localProvider = new LocalProvider(dataRepository.Object);
            var routeProcessor = new Mock<IRouteProcessor>();
            routeProcessor.Setup(x => x.IsMatch(It.IsAny<string>())).Returns(true);
            routeProcessor.Setup(x => x.Process(It.IsAny<HttpRequestMessage>())).ReturnsAsync(() => Result.Ok(new HttpResponseMessage(HttpStatusCode.OK)));
            localProvider.RouteProcessorContainer.AddRouteProcessor(routeProcessor.Object);
            var client = localProvider.HttpMessageHandler.ToHttpClient();
            var response = await client.GetAsync("http://text.example.com");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}