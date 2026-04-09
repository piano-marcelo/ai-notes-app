using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NotesApi.Features.Notes;
using NotesApi.Infrastructure;

namespace NotesApi.Tests.Features.Notes;

[NotInParallel]
[ClassDataSource<NotesApiFactory>(Shared = SharedType.PerTestSession)]
public class DeleteNoteTests(NotesApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Before(Test)]
    public async Task ClearDatabase()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Notes.RemoveRange(db.Notes.IgnoreQueryFilters());
        await db.SaveChangesAsync();
    }

    [Test]
    public async Task DeleteNote_Returns204_AndSetsIsActiveFalse()
    {
        var noteId = await CreateNoteAsync();

        var response = await _client.DeleteAsync($"/api/notes/{noteId}");

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var note = await db.Notes.IgnoreQueryFilters().FirstOrDefaultAsync(n => n.Id == noteId);

        await Assert.That(note).IsNotNull();
        await Assert.That(note!.IsActive).IsFalse();
    }

    [Test]
    public async Task DeleteNote_Returns404_WhenNoteNotFound()
    {
        var response = await _client.DeleteAsync($"/api/notes/{Guid.NewGuid()}");

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task DeleteNote_Returns404_WhenNoteAlreadyInactive()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var note = new Note { Id = Guid.CreateVersion7(), Title = "t", Content = "c", CreatedAtUtc = DateTime.UtcNow, IsActive = false };
        db.Notes.Add(note);
        await db.SaveChangesAsync();

        var response = await _client.DeleteAsync($"/api/notes/{note.Id}");

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task DeleteNote_NoteNoLongerAppearsInGetNotes()
    {
        var noteId = await CreateNoteAsync();

        await _client.DeleteAsync($"/api/notes/{noteId}");

        var getResponse = await _client.GetAsync("/api/notes");
        var notes = await getResponse.Content.ReadFromJsonAsync<List<Note>>();

        await Assert.That(notes!.Any(n => n.Id == noteId)).IsFalse();
    }

    private async Task<Guid> CreateNoteAsync()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var note = new Note { Id = Guid.CreateVersion7(), Title = "To Delete", Content = "Content", CreatedAtUtc = DateTime.UtcNow, IsActive = true };
        db.Notes.Add(note);
        await db.SaveChangesAsync();
        return note.Id;
    }
}
