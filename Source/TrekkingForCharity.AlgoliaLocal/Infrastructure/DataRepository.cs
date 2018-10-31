// Copyright (c) Trekking for Charity. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using LiteDB;

namespace TrekkingForCharity.AlgoliaLocal.Infrastructure
{
    public class DataRepository : IDataRepository
    {
        private readonly string _dataFile;

        public DataRepository()
            : this("algolia_cache.db")
        {
        }

        public DataRepository(string dataFile)
        {
            this._dataFile = dataFile;
        }

        public string Add(string indexName, string jsonObject)
        {
            var data = JsonSerializer.Deserialize(jsonObject).AsDocument;
            if (!data.ContainsKey("objectID"))
            {
                data["objectID"] = Guid.NewGuid().ToString();
            }

            using (var db = new LiteDatabase(this._dataFile))
            {
                var collection = db.GetCollection(indexName);
                collection.Insert(data);

                return data["objectID"];
            }
        }

        public void Update(string index, string objId, string jsonObject)
        {
            using (var db = new LiteDatabase(this._dataFile))
            {
                var collection = db.GetCollection(index);
                var doc = collection.FindOne(query: Query.EQ("objectID", objId));
                var data = JsonSerializer.Deserialize(jsonObject).AsDocument;
                foreach (var keyValuePair in data)
                {
                    doc[keyValuePair.Key] = keyValuePair.Value;
                }

                collection.Update(doc);
            }
        }
    }
}