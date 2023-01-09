using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CalendarWebApi.Api.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CalendarWebApi.Api.Repositories.Users
{
    public class MongoDbUsersRepository : IUsersRepository
    {
        private const string databaseName = "calendar";
        private const string collectionName = "users";
         private readonly IMongoCollection<User> usersCollection;
        private readonly FilterDefinitionBuilder<User> filterBuilder = Builders<User>.Filter;

        public MongoDbUsersRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(databaseName);
            usersCollection = database.GetCollection<User>(collectionName);
        }

        public Task CreateUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task DeleteUserAsync(Guid id)
        {
            throw new NotImplementedException();
        }

         public async Task<User> GetUserAsync(Guid id)
        {
            var filter = filterBuilder.Eq(user => user.Id, id);
            return await usersCollection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task<User> GetUserByUsernameAsync(String username)
        {
            var filter = filterBuilder.Eq(user => user.Username, username);
            return await usersCollection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await usersCollection.Find(new BsonDocument()).ToListAsync();
        }


        public Task UpdateUserAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}