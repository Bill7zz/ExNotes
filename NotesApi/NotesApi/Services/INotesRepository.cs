using System.Collections.Generic;
using System.Threading.Tasks;
using NotesApi.Models;

namespace NotesApi.Services
{    
    public interface INotesRepository<T> where T : class
    {
        Task<IEnumerable<Note>> GetNotesAsync();
        Task<Note> GetNoteAsync(string id, string category);
        Task AddNoteAsync(Note note);
        Task UpdateNoteAsync(string id, Note note);
        Task DeleteNoteAsync(string id, string category);
    }
}
