using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NotesApi.Services;
using NotesApi.Models;
using Microsoft.Azure.Cosmos;

namespace NotesApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly INotesRepository<NotesRepository> _cosmosDbService;

        public NotesController(INotesRepository<NotesRepository> cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [HttpGet]
        public async Task<IEnumerable<Note>> Get()
        {
            return await _cosmosDbService.GetNotesAsync();
        }

        [HttpGet("{category}/{id}")]
        public async Task<ActionResult<Note>> Get(string category, string id)
        {
            Note note = await _cosmosDbService.GetNoteAsync(id, category);

            if (note == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(note);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Note note)
        {
            try
            {
                await _cosmosDbService.AddNoteAsync(note);
                return Ok();
            }
            catch (CosmosException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpDelete("{category}/{id}")]
        public async Task<ActionResult<Note>> Delete(string category, string id)
        {
            try
            {
                await _cosmosDbService.DeleteNoteAsync(id, category);
                return Ok();
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
