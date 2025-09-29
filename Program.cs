using Microsoft.EntityFrameworkCore;
using todosomethinglarger.Data;
using todosomethinglarger.Models;

var builder = WebApplication.CreateBuilder(args);

// DbContext: pekar mot SQL Server-containern "db"
builder.Services.AddDbContext<TodoContext>(opt =>
    opt.UseSqlServer(
        "Server=db;Database=TodoApp;User Id=sa;Password=Your_password123;TrustServerCertificate=True;",
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()
    ));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger alltid aktiv
app.UseSwagger();
app.UseSwaggerUI();

// HTTPS redirect (kan tas bort om du vill testa enklare lokalt)
app.UseHttpsRedirection();

// === CRUD endpoints ===
app.MapGet("/todos", async (TodoContext db) =>
    await db.Todos.ToListAsync());

app.MapPost("/todos", async (Todo todo, TodoContext db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todos/{todo.Id}", todo);
});

app.MapDelete("/todos/{id}", async (int id, TodoContext db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    db.Todos.Remove(todo);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// === Automatiska migrationer + seed ===
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TodoContext>();
    try
    {
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Migration misslyckades: {ex.Message}");
    }


    if (!db.Todos.Any())
    {
        db.Todos.AddRange(
            new Todo { Title = "Lära mig Docker", IsDone = false },
            new Todo { Title = "Bygga klart TodoApp", IsDone = false },
            new Todo { Title = "Testa Swagger", IsDone = true }
        );
        db.SaveChanges();
    }
}

app.Run();
