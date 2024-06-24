using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Taskregister.Server.Persistance;
using Taskregister.Server.Task.Repository;
using Taskregister.Server.Task.Services;
using Taskregister.Server.User.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TaskRegisterDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("TaskRegisterDb")));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();
var appliactionAssembly = typeof(Program).Assembly;
builder.Services.AddValidatorsFromAssembly(appliactionAssembly).AddFluentValidationAutoValidation();
//builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options=>options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));   

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
