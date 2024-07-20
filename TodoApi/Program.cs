using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

//Add Dependency Injections - Addservices
builder.Services.AddDbContext<ToDoDbContext>(option => option.UseInMemoryDatabase("TodoList"));

var app = builder.Build();

//Configure pipeline - UseMethods (to configure request lifecycles)

app.MapGet("/todoitems", async (ToDoDbContext context) => 
  await context.TodoItems.ToListAsync());


app.MapGet("/todoitems/{id}", async (int id, ToDoDbContext context) => 
   await context.TodoItems.FindAsync(id));

app.MapPost("/todoitems", async (TodoItem todoItem, ToDoDbContext context) =>
{
    context.TodoItems.Add(todoItem);
    await context.SaveChangesAsync();
    return Results.Created($"/todoitems/{todoItem.Id}", todoItem);
});

app.MapPut("/todoitems/{id}", async (int id, TodoItem todoItem, ToDoDbContext context) =>
{
    var todoItemExist = await context.TodoItems.FindAsync(id);
    if (todoItemExist is null)
        return Results.NotFound();
    todoItemExist.Name = todoItem.Name;
    todoItemExist.IsCompleted = todoItem.IsCompleted;
    todoItemExist.Description = todoItem.Description;
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/todoitems/{id}", async (int id, ToDoDbContext context) =>
{ 
   if(await context.TodoItems.FindAsync(id) is TodoItem todoItem)
    {
        context.TodoItems.Remove(todoItem);
        await context.SaveChangesAsync();
        return Results.NoContent();
    }
   return Results.NotFound();
});

app.Run();
