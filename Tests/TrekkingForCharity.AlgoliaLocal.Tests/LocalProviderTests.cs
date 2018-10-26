// Copyright (c) Trekking for Charity. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Xunit;

namespace TrekkingForCharity.AlgoliaLocal.Tests
{
    public class LocalProviderTests
    {
        [Fact]
        public void WhenClassIsInitialized_MockHandlerIsCreated()
        {
            var localProvider = new LocalProvider();

            Assert.NotNull(localProvider.HttpMessageHandler);
        }

        [Fact]
        public async Task WhenLookingForAnUnknownRoute_ExceptionIsThrown()
        {
            var localProvider = new LocalProvider();

            var client = localProvider.HttpMessageHandler.ToHttpClient();
            var exception =
                await Assert.ThrowsAsync<NotImplementedException>(() => client.GetAsync("http://text.example.com"));
            Assert.Equal("No route setup for / with the method GET", exception.Message);
        }
    }
}