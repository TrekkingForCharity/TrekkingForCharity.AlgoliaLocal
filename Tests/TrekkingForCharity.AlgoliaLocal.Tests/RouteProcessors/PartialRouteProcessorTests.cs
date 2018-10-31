using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Moq;
using TrekkingForCharity.AlgoliaLocal.Infrastructure;
using TrekkingForCharity.AlgoliaLocal.RouteProcessors;
using Xunit;

namespace TrekkingForCharity.AlgoliaLocal.Tests.RouteProcessors
{
    public class PartialRouteProcessorTests
    {
        [Fact]
        public async Task Process_WhenMethodIsNotPut_ExpectFailedResult()
        {
            var dataRep = new Mock<IDataRepository>();
            var processor = new PartialRouteProcessor(dataRep.Object);

            var message = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://test.example.com/1/indexes/test-index/obj1/partial")
            };

            var result = await processor.Process(message);

            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task Process_WhenPathDoesNotMatch_ExpectFailedResult()
        {
            var dataRep = new Mock<IDataRepository>();
            var processor = new PartialRouteProcessor(dataRep.Object);

            var message = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri("https://test.example.com/1/es/test-index")
            };

            var result = await processor.Process(message);

            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task Process_WhenValid_ResultWithMessage()
        {
            var dataRep = new Mock<IDataRepository>();
            var processor = new PartialRouteProcessor(dataRep.Object);

            var message = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri("https://test.example.com/1/indexes/test-index/obj1/partial"),
                Content = new StringContent(string.Empty)
            };

            var result = await processor.Process(message);

            Assert.True(result.IsSuccess);
        }
    }
}
