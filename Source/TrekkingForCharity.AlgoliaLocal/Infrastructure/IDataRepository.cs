// Copyright (c) Trekking for Charity. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Net.Http;
using System.Threading.Tasks;

namespace TrekkingForCharity.AlgoliaLocal.Infrastructure
{
    public interface IDataRepository
    {
        string Add(string indexName, string jsonObject);
    }
}