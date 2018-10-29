// Copyright (c) Trekking for Charity. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ResultMonad;

namespace TrekkingForCharity.AlgoliaLocal.Infrastructure
{
    public sealed class RouteProcessorContainer
    {
        private readonly List<IRouteProcessor> _customRouteProcessors;
        private readonly List<IRouteProcessor> _routeProcessors;

        public RouteProcessorContainer(IDataRepository dataRepository)
        {
            this._customRouteProcessors = new List<IRouteProcessor>();
            this._routeProcessors = new List<IRouteProcessor>();

            var assembly = typeof(IRouteProcessor).GetTypeInfo().Assembly;

            var types = assembly.DefinedTypes.Where(x => !x.IsAbstract && x.ImplementedInterfaces.Contains(typeof(IRouteProcessor)));

            foreach (var typeInfo in types)
            {
                this._routeProcessors.Add(
                    (IRouteProcessor)Activator.CreateInstance(typeInfo.AsType(), dataRepository));
            }
        }

        public IReadOnlyList<IRouteProcessor> RouteProcessors => this._routeProcessors.AsReadOnly();

        public IReadOnlyList<IRouteProcessor> CustomRouteProcessors => this._customRouteProcessors.AsReadOnly();

        public Result<IRouteProcessor> GetRouteForPath(string absolutePath)
        {
            foreach (var routeProcessor in this._customRouteProcessors)
            {
                if (routeProcessor.IsMatch(absolutePath))
                {
                    return Result.Ok(routeProcessor);
                }
            }

            foreach (var routeProcessor in this._routeProcessors)
            {
                if (routeProcessor.IsMatch(absolutePath))
                {
                    return Result.Ok(routeProcessor);
                }
            }

            return Result.Fail<IRouteProcessor>();
        }

        public void AddRouteProcessor(IRouteProcessor routeProcessor)
        {
            this._customRouteProcessors.Add(routeProcessor);
        }
    }
}