// Copyright (c) Trekking for Charity. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TrekkingForCharity.AlgoliaLocal.Infrastructure
{
    public sealed class RouteProcessorList
    {
        private readonly List<IRouteProcessor> _routeProcessors;

        public RouteProcessorList()
        {
            this._routeProcessors = new List<IRouteProcessor>();

            var assembly = typeof(IRouteProcessor).GetTypeInfo().Assembly;

            var types = assembly.DefinedTypes.Where(x => x.ImplementedInterfaces.Contains(typeof(IRouteProcessor)));

            foreach (var typeInfo in types)
            {
                this._routeProcessors.Add((IRouteProcessor)Activator.CreateInstance(typeInfo.AsType()));
            }
        }

        public IReadOnlyList<IRouteProcessor> RouteProcessors => this._routeProcessors.AsReadOnly();

        public IRouteProcessor GetRouteForPath(string absolutePath)
        {
            foreach (var routeProcessor in this._routeProcessors)
            {
                if (routeProcessor.Route.IsMatch(absolutePath))
                {
                    return routeProcessor;
                }
            }

            throw new NotImplementedException($"No route setup for {absolutePath}");
        }
    }
}