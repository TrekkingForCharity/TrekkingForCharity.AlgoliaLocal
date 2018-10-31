// Copyright (c) Trekking for Charity. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;
using LiteDB;
using Newtonsoft.Json;
using TrekkingForCharity.AlgoliaLocal.Infrastructure;
using Xunit;

namespace TrekkingForCharity.AlgoliaLocal.Tests.Infrastructure
{
    public class DataRepositoryTests
    {
        private readonly string _fileName;

        private readonly string _index = "dummy_index";

        public DataRepositoryTests()
        {
            this._fileName = $"DataRepositoryTests_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.db";
        }

        [Fact]
        public void Add_WhenNoObjectId_ExpectOneIsAssignedAndDataSaved()
        {
            var dataRepo = new DataRepository(this._fileName);
            var dataToSend = new
            {
                FirstName = "Test",
                LastName = "Testing"
            };
            var objectReturn = dataRepo.Add(this._index, JsonConvert.SerializeObject(dataToSend));

            Assert.True(Guid.TryParse(objectReturn, out _));

            using (var db = new LiteDatabase(this._fileName))
            {
                Query.EQ("objectID", objectReturn);
                var collection = db.GetCollection(this._index);
                var doc = collection.FindOne(Query.EQ("objectID", objectReturn));
                Assert.NotEmpty(doc);
            }
        }

        [Fact]
        public void Add_WhenObjectIdIsAssigned_ExpectDataSaved()
        {
            var dataRepo = new DataRepository(this._fileName);
            var objId = $"SomeId_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
            var dataToSend = new
            {
                FirstName = "Test",
                LastName = "Testing",
                objectID = objId
            };
            var objectReturn = dataRepo.Add(this._index, JsonConvert.SerializeObject(dataToSend));

            Assert.Equal(objId, objectReturn);

            using (var db = new LiteDatabase(this._fileName))
            {
                Query.EQ("objectID", objectReturn);
                var collection = db.GetCollection(this._index);
                var doc = collection.FindOne(Query.EQ("objectID", objectReturn));
                Assert.NotEmpty(doc);
            }
        }

        [Fact]
        public void Update_WhenValid_ExpectDataSaved()
        {
            var objId = $"SomeId_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
            using (var db = new LiteDatabase(this._fileName))
            {
                var collection = db.GetCollection(this._index);
                var seedData = new BsonDocument();
                seedData["FirstName"] = "Before";
                seedData["LastName"] = "Change";
                seedData["objectID"] = objId;
                collection.Insert(seedData);
            }

            var dataToSend = new
            {
                FirstName = "After",
                LastName = "Changes",
                objectID = objId
            };
            var dataRepo = new DataRepository(this._fileName);
            dataRepo.Update(this._index, objId, JsonConvert.SerializeObject(dataToSend));


            using (var db = new LiteDatabase(this._fileName))
            {
                var collection = db.GetCollection(this._index);
                var doc = collection.FindOne(Query.EQ("objectID", objId));
                Assert.Equal("After", doc["FirstName"]);
                Assert.Equal("Changes", doc["LastName"]);
            }
        }
    }
}