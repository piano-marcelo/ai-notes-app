using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NotesApi.Features.Notes;
using NotesApi.Infrastructure;

namespace NotesApi.Tests.Features.Notes;

[NotInParallel]
[ClassDataSource<NotesApiFactory>(Shared = SharedType.PerTestSession)]
public class UpdateNoteTests(NotesApiFactory factory)
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
    public async Task UpdateNote_Returns200_WithUpdatedNote()
    {
        var noteId = await CreateNoteAsync("Original Title", "Original Content");
        var request = new { title = "Updated Title", content = "Updated Content" };

        var response = await _client.PutAsJsonAsync($"/api/notes/{noteId}", request);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);

        var note = await response.Content.ReadFromJsonAsync<Note>();
        await Assert.That(note!.Title).IsEqualTo("Updated Title");
        await Assert.That(note.Content).IsEqualTo("Updated Content");
        await Assert.That(note.UpdatedAtUtc).IsNotNull();
    }

    [Test]
    public async Task UpdateNote_Returns404_WhenNoteNotFound()
    {
        var request = new { title = "Title", content = "Content" };

        var response = await _client.PutAsJsonAsync($"/api/notes/{Guid.NewGuid()}", request);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task UpdateNote_Returns422_WhenTitleExceeds150Characters()
    {
        var noteId = await CreateNoteAsync("Title", "Content");
        var request = new { title = new string('a', 151), content = "Content" };

        var response = await _client.PutAsJsonAsync($"/api/notes/{noteId}", request);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task UpdateNote_Returns404_WhenNoteIsInactive()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var note = new Note { Id = Guid.CreateVersion7(), Title = "t", Content = "c", CreatedAtUtc = DateTime.UtcNow, IsActive = false };
        db.Notes.Add(note);
        await db.SaveChangesAsync();

        var request = new { title = "New Title", content = "Content" };
        var response = await _client.PutAsJsonAsync($"/api/notes/{note.Id}", request);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    private async Task<Guid> CreateNoteAsync(string title, string content)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var note = new Note { Id = Guid.CreateVersion7(), Title = title, Content = content, CreatedAtUtc = DateTime.UtcNow, IsActive = true };
        db.Notes.Add(note);
        await db.SaveChangesAsync();
        return note.Id;
    }
}
