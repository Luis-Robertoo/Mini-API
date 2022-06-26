using MiniTodo.Data;
using MiniTodo.ViewModels;

var MyAllowSpecificOrigins = "_MyAllowSubdomainPolicy";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:8080").AllowAnyHeader()
                                                  .AllowAnyMethod();

                      });
});


builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(MyAllowSpecificOrigins);

app.MapGet("v1/todos", (AppDbContext context) =>
{
    var todos = context.Todos.ToList();
    return Results.Ok(todos);
}).Produces<Todo>();

app.MapPost("v1/todos", (
    AppDbContext context,
    CreateTodoViewModel model) =>
{
    var todo = model.MapTo();

    if (!model.IsValid)
    {
        return Results.BadRequest(model.Notifications);
    }
    context.Todos.Add(todo);
    context.SaveChanges();

    return Results.Created($"/v1/todos/{todo.Id}", todo);

});

app.Run();
