using NotesApi.Infrastructure;

namespace NotesApi.Features.Notes;

public static class UpdateNote
{
    public record UpdateNoteRequest(string Title, string Content);

    public static IEndpointRouteBuilder MapUpdateNote(this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/notes/{id:guid}", async (Guid id, UpdateNoteRequest request, AppDbContext db) =>
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

            var note = await db.Notes.FindAsync(id);
            if (note is null)
                return Results.NotFound();

            note.Title = request.Title.Trim();
            note.Content = request.Content.Trim();
            note.UpdatedAtUtc = DateTime.UtcNow;

            await db.SaveChangesAsync();

            return Results.Ok(note);
        });

        return app;
    }
}
