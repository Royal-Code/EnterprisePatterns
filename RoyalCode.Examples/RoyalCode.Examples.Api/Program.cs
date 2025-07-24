using RoyalCode.Examples.Blogs.Api;
using RoyalCode.Examples.Blogs.Infra.Persistence;
using RoyalCode.SmartCommands.WorkContext.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddBlobs();

builder.Services.AddSqliteInMemoryWorkContextDefault()
    .EnsureDatabaseCreated()
    .ConfigureBlogs()
    .AddUnitOfWorkAdapter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapBlob();

app.Run();
