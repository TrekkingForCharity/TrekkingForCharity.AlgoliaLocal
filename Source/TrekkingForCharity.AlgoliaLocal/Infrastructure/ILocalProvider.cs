// Copyright (c) Trekking for Charity. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using RichardSzalay.MockHttp;

namespace TrekkingForCharity.AlgoliaLocal.Infrastructure
{
    public interface ILocalProvider
    {
        MockHttpMessageHandler HttpMessageHandler { get; }
    }
}