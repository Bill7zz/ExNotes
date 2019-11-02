using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using NotesApi.Models;
using System.Threading.Tasks;

namespace NotesApi.Services
{
    public class NotesRepository : INotesRepository<NotesRepository>
    {
        private Container _container;

        public NotesRepository(CosmosClient dbClient, string databaseName, string containerName)
        {
            this._container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task AddNoteAsync(Note note)
        {
            await this._container.CreateItemAsync<Note>(note, new PartitionKey(note.Category));
        }

        public async Task DeleteNoteAsync(string id, string category)
        {
            await _container.DeleteItemAsync<Note>(id, new PartitionKey(category));
        }

        public async Task<Note> GetNoteAsync(string id, string category)
        {
            try
            {
                ItemResponse<Note> response = await this._container.ReadItemAsync<Note>(id, new PartitionKey(category));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }
        
        public async Task<IEnumerable<Note>> GetNotesAsync()
        {
            var query = this._container.GetItemQueryIterator<Note>(new QueryDefinition("Select * From c"));
            List<Note> results = new List<Note>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task UpdateNoteAsync(string id, Note Note)
        {
            await this._container.UpsertItemAsync<Note>(Note, new PartitionKey(id));
        }
    }
}
