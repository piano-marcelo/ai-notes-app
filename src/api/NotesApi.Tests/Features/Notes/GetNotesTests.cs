using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NotesApi.Features.Notes;
using NotesApi.Infrastructure;

namespace NotesApi.Tests.Features.Notes;

[NotInParallel]
[ClassDataSource<NotesApiFactory>(Shared = SharedType.PerTestSession)]
public class GetNotesTests(NotesApiFactory factory)
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
    public async Task GetNotes_Returns200_WithEmptyList_WhenNoNotesExist()
    {
        var response = await _client.GetAsync("/api/notes");

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);

        var notes = await response.Content.ReadFromJsonAsync<List<Note>>();
        await Assert.That(notes).IsNotNull();
        await Assert.That(notes!.Count).IsEqualTo(0);
    }

    [Test]
    public async Task GetNotes_ReturnsOnlyActiveNotes()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.Notes.AddRange(
            new Note { Id = Guid.CreateVersion7(), Title = "Active Note", Content = "Content", CreatedAtUtc = DateTime.UtcNow, IsActive = true },
            new Note { Id = Guid.CreateVersion7(), Title = "Inactive Note", Content = "Content", CreatedAtUtc = DateTime.UtcNow, IsActive = false }
        );
        await db.SaveChangesAsync();

        var response = await _client.GetAsync("/api/notes");
        var notes = await response.Content.ReadFromJsonAsync<List<Note>>();

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(notes!.Count).IsEqualTo(1);
        await Assert.That(notes[0].Title).IsEqualTo("Active Note");
    }

    [Test]
    public async Task GetNotes_ReturnsNotesOrderedByCreatedAtUtc()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var older = new Note { Id = Guid.CreateVersion7(), Title = "Older", Content = "c", CreatedAtUtc = DateTime.UtcNow.AddHours(-1), IsActive = true };
        var newer = new Note { Id = Guid.CreateVersion7(), Title = "Newer", Content = "c", CreatedAtUtc = DateTime.UtcNow, IsActive = true };
        db.Notes.AddRange(newer, older);
        await db.SaveChangesAsync();

        var response = await _client.GetAsync("/api/notes");
        var notes = await response.Content.ReadFromJsonAsync<List<Note>>();

        await Assert.That(notes![0].Title).IsEqualTo("Older");
        await Assert.That(notes[1].Title).IsEqualTo("Newer");
    }
}
