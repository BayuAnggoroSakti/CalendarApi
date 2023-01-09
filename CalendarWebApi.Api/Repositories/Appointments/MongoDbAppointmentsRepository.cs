using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CalendarWebApi.Api.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CalendarWebApi.Api.Repositories.Appointments
{
    public class MongoDbAppointmentsRepository : IAppointmentsRepository
    {
        private const string databaseName = "calendar";
        private const string collectionName = "appointments";
        private readonly IMongoCollection<Appointment> appointmentsCollection;
        private readonly FilterDefinitionBuilder<Appointment> filterBuilder = Builders<Appointment>.Filter;

        public MongoDbAppointmentsRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(databaseName);
            appointmentsCollection = database.GetCollection<Appointment>(collectionName);
        }

        public async Task CreateAppointmentAsync(Appointment appointment)
        {
            await appointmentsCollection.InsertOneAsync(appointment);
        }

        public async Task<Appointment> CheckAppointmentConflictAsync(String username, DateTime? startDate, DateTime? endDate)
        {
            return await appointmentsCollection.Find( 
                x => 
                ((startDate >= x.StartDate &
                startDate <= x.EndDate) ||
                (endDate >= x.StartDate &
                endDate <= x.EndDate)) & 
                x.Username == username
                ).SingleOrDefaultAsync();
        }

        public async Task<Appointment> CheckAppointmentConflictWithIdAsync(Guid id,String username, DateTime? startDate, DateTime? endDate)
        {
            return await appointmentsCollection.Find( 
                x => 
                ((startDate >= x.StartDate &
                startDate <= x.EndDate) ||
                (endDate >= x.StartDate &
                endDate <= x.EndDate)) & 
                x.Username == username & 
                x.Id != id
                ).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsAsync()
        {
            return await appointmentsCollection.Find(new BsonDocument()).ToListAsync();
        }

         public async Task<Appointment> GetAppointmentsAsync(Guid id)
        {
            var filter = filterBuilder.Eq(appointment => appointment.Id, id);
            return await appointmentsCollection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task UpdateAppointmentAsync(Appointment appointment)
        {
            var filter = filterBuilder.Eq(existingAppontment => existingAppontment.Id, appointment.Id);
            await appointmentsCollection.ReplaceOneAsync(filter, appointment);
        }

        public async Task DeleteAppointmentAsync(Guid id)
        {
            var filter = filterBuilder.Eq(appointment => appointment.Id, id);
            await appointmentsCollection.DeleteOneAsync(filter);
        }
    }
}