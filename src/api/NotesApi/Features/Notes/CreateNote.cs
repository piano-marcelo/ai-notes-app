using NotesApi.Infrastructure;

namespace NotesApi.Features.Notes;

public static class CreateNote
{
    public record CreateNoteRequest(string Title, string Content);

    public static IEndpointRouteBuilder MapCreateNote(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/notes", async (CreateNoteRequest request, AppDbContext db) =>
        {
            if (string.IsNullOrWhiteSpace(request.Title) || request.Title.Length > 150)
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["title"] = ["Title is required and must be at most 150 characters."]
                });

            if (string.IsNullOrWhiteSpace(request.Content) || request.Content.Length > 2000)
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["content"] = ["Content is required and must be at most 2000 characters."]
                });

            var note = new Note
            {
                Id = Guid.CreateVersion7(),
                Title = request.Title.Trim(),
                Content = request.Content.Trim(),
                CreatedAtUtc = DateTime.UtcNow,
                IsActive = true
            };

            db.Notes.Add(note);
            await db.SaveChangesAsync();

            return Results.Created($"/api/notes/{note.Id}", note);
        });

        return app;
    }
}
