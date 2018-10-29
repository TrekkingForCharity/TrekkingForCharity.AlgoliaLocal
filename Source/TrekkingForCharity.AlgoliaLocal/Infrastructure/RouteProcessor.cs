// Copyright (c) Trekking for Charity. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ResultMonad;

namespace TrekkingForCharity.AlgoliaLocal.Infrastructure
{
    public abstract class RouteProcessor : IRouteProcessor
    {
        public abstract Regex Route { get; }

        public bool IsMatch(string path) => this.Route.IsMatch(path);

        public abstract Task<Result<HttpResponseMessage>> Process(HttpRequestMessage message);
    }
}