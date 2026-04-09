using NotesApi.Infrastructure;

namespace NotesApi.Features.Notes;

public static class DeleteNote
{
    public static IEndpointRouteBuilder MapDeleteNote(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/notes/{id:guid}", async (Guid id, AppDbContext db) =>
        {
            var note = await db.Notes.FindAsync(id);
            if (note is null)
                return Results.NotFound();

            note.IsActive = false;
            await db.SaveChangesAsync();

            return Results.NoContent();
        });

        return app;
    }
}
