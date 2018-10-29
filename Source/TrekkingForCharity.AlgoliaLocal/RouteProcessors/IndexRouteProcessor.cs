// Copyright (c) Trekking for Charity. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ResultMonad;
using TrekkingForCharity.AlgoliaLocal.Infrastructure;

namespace TrekkingForCharity.AlgoliaLocal.RouteProcessors
{
    public class IndexRouteProcessor : RouteProcessor
    {
        private readonly IDataRepository _dataRepository;

        public IndexRouteProcessor(IDataRepository dataRepository)
        {
            this._dataRepository = dataRepository;
        }

        public override Regex Route { get; } = new Regex("^/1/indexes/(?<index>[^/]*)$", RegexOptions.IgnoreCase);

        public override async Task<Result<HttpResponseMessage>> Process(HttpRequestMessage message)
        {
            var path = message.RequestUri.AbsolutePath;

            var match = this.Route.Match(path);
            if (!match.Success)
            {
                return Result.Fail<HttpResponseMessage>();
            }

            if (message.Method != HttpMethod.Post)
            {
                return Result.Fail<HttpResponseMessage>();
            }

            var json = await message.Content.ReadAsStringAsync();
            var id = this._dataRepository.Add(match.Groups["index"].Value, json);

            return Result.Ok(new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent(
                    JsonConvert.SerializeObject(new
                    {
                        createdAt = DateTime.UtcNow,
                        taskId = 1,
                        objectID = id
                    }), Encoding.UTF8, "application/json")
            });
        }
    }
}