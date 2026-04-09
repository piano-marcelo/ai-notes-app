using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NotesApi.Features.Notes;
using NotesApi.Infrastructure;

namespace NotesApi.Tests.Features.Notes;

[NotInParallel]
[ClassDataSource<NotesApiFactory>(Shared = SharedType.PerTestSession)]
public class CreateNoteTests(NotesApiFactory factory)
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
    public async Task CreateNote_Returns201_WithCreatedNote()
    {
        var request = new { title = "Test Note", content = "Test Content" };

        var response = await _client.PostAsJsonAsync("/api/notes", request);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);

        var note = await response.Content.ReadFromJsonAsync<Note>();
        await Assert.That(note).IsNotNull();
        await Assert.That(note!.Title).IsEqualTo("Test Note");
        await Assert.That(note.Content).IsEqualTo("Test Content");
        await Assert.That(note.IsActive).IsTrue();
        await Assert.That(note.Id).IsNotEqualTo(Guid.Empty);
    }

    [Test]
    public async Task CreateNote_Returns400_WhenTitleIsEmpty()
    {
        var request = new { title = "", content = "Content" };

        var response = await _client.PostAsJsonAsync("/api/notes", request);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task CreateNote_Returns400_WhenTitleExceeds150Characters()
    {
        var request = new { title = new string('a', 151), content = "Content" };

        var response = await _client.PostAsJsonAsync("/api/notes", request);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task CreateNote_Returns400_WhenContentExceeds2000Characters()
    {
        var request = new { title = "Title", content = new string('a', 2001) };

        var response = await _client.PostAsJsonAsync("/api/notes", request);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task CreateNote_Returns400_WhenContentIsEmpty()
    {
        var request = new { title = "Title", content = "" };

        var response = await _client.PostAsJsonAsync("/api/notes", request);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }
}
