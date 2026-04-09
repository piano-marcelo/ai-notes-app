using Microsoft.EntityFrameworkCore;
using NotesApi.Infrastructure;

namespace NotesApi.Features.Notes;

public static class GetNotes
{
    public static IEndpointRouteBuilder MapGetNotes(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/notes", async (AppDbContext db) =>
        {
            var notes = await db.Notes
                .OrderBy(n => n.CreatedAtUtc)
                .ToListAsync();
            return Results.Ok(notes);
        });

        return app;
    }
}
