// Copyright (c) Trekking for Charity. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using TrekkingForCharity.AlgoliaLocal.Infrastructure;
using TrekkingForCharity.AlgoliaLocal.RouteProcessors;
using Xunit;

namespace TrekkingForCharity.AlgoliaLocal.Tests.RouteProcessors
{
    public class IndexRouteProcessorTests
    {
        [Fact]
        public async Task Process_WhenMethodIsNotPost_ExpectException()
        {
            var dataRep = new Mock<IDataRepository>();
            var processor = new IndexRouteProcessor(dataRep.Object);

            var message = new HttpRequestMessage
            {
                Method = HttpMethod.Get, RequestUri = new Uri("https://test.example.com/1/indexes/test-index")
            };

            var result = await processor.Process(message);

            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task Process_WhenPathDoesNotMatch_ExpectFailedResult()
        {
            var dataRep = new Mock<IDataRepository>();
            var processor = new IndexRouteProcessor(dataRep.Object);

            var message = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://test.example.com/1/es/test-index")
            };

            var result = await processor.Process(message);

            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task Process_WhenValid_ResultWithMessage()
        {
            var dataRep = new Mock<IDataRepository>();
            var processor = new IndexRouteProcessor(dataRep.Object);

            var message = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://test.example.com/1/indexes/test-index"),
                Content = new StringContent(string.Empty)
            };

            var result = await processor.Process(message);

            Assert.True(result.IsSuccess);
        }
    }
}