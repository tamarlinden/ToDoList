using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"),
        new MySqlServerVersion(new Version(8, 0, 32))
    ));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors();

// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }



// Get all items
app.MapGet("/tasks", async (ToDoDbContext context) =>
{
    return await context.Items.ToListAsync();
});

// Create a new item
app.MapPost("/tasks", async (Item newItem, ToDoDbContext context) =>
{
    context.Items.Add(newItem);
    await context.SaveChangesAsync();
    return Results.Created($"/tasks/{newItem.Id}", newItem);
});

// Update an existing item
app.MapPut("/tasks/{id}", async (int id, Item updatedItem, ToDoDbContext context) =>
{
    var item = await context.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    item.Name = updatedItem.Name;
    item.IsComplete = updatedItem.IsComplete;

    await context.SaveChangesAsync();
    return Results.NoContent();
});

// Delete an item
app.MapDelete("/tasks/{id}", async (int id, ToDoDbContext context) =>
{
    var item = await context.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    context.Items.Remove(item);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("./",()=>"AuthServer API: is running")

app.Run();

