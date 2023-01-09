using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CalendarWebApi.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CalendarWebApi.Repositories.Users
{
    public class MongoDbUsersRepository : IUsersRepository
    {
        private const string databaseName = "calendar";
        private const string collectionName = "users";
        public MongoDbUsersRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(databaseName);
        }

        public Task CreateUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task DeleteUserAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetUsersAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateUserAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}