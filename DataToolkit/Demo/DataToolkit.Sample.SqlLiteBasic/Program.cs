using DataToolkit.Library.Extensions;
using DataToolkit.Library.UnitOfWorkLayer;
using DataToolkit.Provider.Sqlite.Extensions;


Microsoft.AspNetCore.Builder.WebApplicationBuilder 
    builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataToolkit(options =>
{
    options.DefaultConnectionAlias = "Sqlite";
});

builder.Services.AddDataToolkitSqlite();

var provider = builder.Services.BuildServiceProvider();
var uow =
    provider.GetRequiredService<IUnitOfWork>();

var rows =
    await uow.Sql.FromSqlAsync<dynamic>(
        "SELECT sqlite_version()");

Console.WriteLine(rows.FirstOrDefault());

var rowsquery =
    await uow.Sql.FromSqlAsync<dynamic>(
        "SELECT * FROM PERSON");

Console.WriteLine(rowsquery.FirstOrDefault());
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
