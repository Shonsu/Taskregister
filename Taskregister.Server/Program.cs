using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Taskregister.Server.Persistence;
using Taskregister.Server.Tags.Repository;
using Taskregister.Server.Tags.Services;
using Taskregister.Server.Todos.Repository;
using Taskregister.Server.Todos.Services;
using Taskregister.Server.User.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TodosRegisterDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("TaskRegisterDb")));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITodosRepository, TodosRepository>();
builder.Services.AddScoped<ITodosService, TodosService>();
builder.Services.AddScoped<ITagsService, TagsService>();
builder.Services.AddScoped<ITagsRepository, TagsRepository>();

var applicationAssembly = typeof(Program).Assembly;
builder.Services.AddValidatorsFromAssembly(applicationAssembly).AddFluentValidationAutoValidation();
// TODO change to without aspnetcore validator
// builder.Services.AddValidatorsFromAssembly(applicationAssembly)
//builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options=>options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",

        builder =>
        {
            builder.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod();
        });
});
var app = builder.Build();

app.UseCors("AllowSpecificOrigin");
app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName.Equals("DevelopmentMsSqlLocalDb"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetService<TodosRegisterDbContext>();
IEnumerable<string> pendingMigrations = dbContext.Database.GetPendingMigrations();
if (pendingMigrations.Any())
{
    dbContext.Database.Migrate();
}

app.UseAuthorization();

app.MapControllers();

//app.MapFallbackToFile("/index.html");

app.Run();