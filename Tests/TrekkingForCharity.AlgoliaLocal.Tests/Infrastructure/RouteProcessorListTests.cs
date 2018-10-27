// Copyright (c) Trekking for Charity. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using TrekkingForCharity.AlgoliaLocal.Infrastructure;
using Xunit;

namespace TrekkingForCharity.AlgoliaLocal.Tests.Infrastructure
{
    public class RouteProcessorListTests
    {
        [Fact]
        public void WhenClassIsInitialized_RouteProcessorsAreDiscoveredCorrectly()
        {
            var routeProcessorList = new RouteProcessorList();
            Assert.Equal(0, routeProcessorList.RouteProcessors.Count);
        }

        [Fact]
        public void WhenRouteIsNotFound_ExceptionIsThrown()
        {
            var routeProcessorList = new RouteProcessorList();
            var exception = Assert.Throws<NotImplementedException>(() => routeProcessorList.GetRouteForPath("/"));
            Assert.Equal("No route setup for /", exception.Message);
        }
    }
}