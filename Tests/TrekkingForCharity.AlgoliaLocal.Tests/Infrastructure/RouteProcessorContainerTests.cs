// Copyright (c) Trekking for Charity. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Moq;
using TrekkingForCharity.AlgoliaLocal.Infrastructure;
using TrekkingForCharity.AlgoliaLocal.RouteProcessors;
using Xunit;

namespace TrekkingForCharity.AlgoliaLocal.Tests.Infrastructure
{
    public class RouteProcessorContainerTests
    {
        [Fact]
        public void WhenClassIsInitialized_RouteProcessorsAreDiscoveredCorrectly()
        {
            var dataRepository = new Mock<IDataRepository>();
            var routeProcessorList = new RouteProcessorContainer(dataRepository.Object);
            Assert.Equal(1, routeProcessorList.RouteProcessors.Count);
            Assert.Empty(routeProcessorList.CustomRouteProcessors);
        }

        [Fact]
        public void GetRouteForPath_WhenRouteIsNotFound_FailedResultIsReturned()
        {
            var dataRepository = new Mock<IDataRepository>();
            var routeProcessorList = new RouteProcessorContainer(dataRepository.Object);
            var routeResult = routeProcessorList.GetRouteForPath("/");
            Assert.True(routeResult.IsFailure);
        }

        [Fact]
        public void GetRouteForPath_WhenCustomRouteIsFound_RouteIsReturned()
        {
            var dataRepository = new Mock<IDataRepository>();
            var routeProcessorList = new RouteProcessorContainer(dataRepository.Object);
            var routeProcessor = new Mock<IRouteProcessor>();
            routeProcessor.Setup(x => x.IsMatch(It.IsAny<string>())).Returns(true);
            routeProcessorList.AddRouteProcessor(routeProcessor.Object);
            var routeResult = routeProcessorList.GetRouteForPath("/");
            Assert.True(routeResult.IsSuccess);
            Assert.Equal(routeResult.Value, routeProcessor.Object);
        }

        [Fact]
        public void GetRouteForPath_WhenRouteIsFound_RouteIsReturned()
        {
            var dataRepository = new Mock<IDataRepository>();
            var routeProcessorList = new RouteProcessorContainer(dataRepository.Object);
            var routeResult = routeProcessorList.GetRouteForPath("/1/indexes/index");
            Assert.True(routeResult.IsSuccess);
            Assert.IsType<IndexRouteProcessor>(routeResult.Value);
        }

        [Fact]
        public void GetRouteForPath_WhenCustomRouteAndRouteSharePath_CustomRouteIsReturned()
        {
            var dataRepository = new Mock<IDataRepository>();
            var routeProcessorList = new RouteProcessorContainer(dataRepository.Object);
            var routeProcessor = new Mock<IRouteProcessor>();
            routeProcessor.Setup(x => x.IsMatch(It.IsAny<string>())).Returns(true);
            routeProcessorList.AddRouteProcessor(routeProcessor.Object);
            var routeResult = routeProcessorList.GetRouteForPath("/1/indexes/index");
            Assert.True(routeResult.IsSuccess);
            Assert.Equal(routeResult.Value, routeProcessor.Object);
        }
    }
}